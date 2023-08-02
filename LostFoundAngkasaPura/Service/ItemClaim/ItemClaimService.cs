using AutoMapper;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemFound;
using LostFoundAngkasaPura.Service.Mailer;
using LostFoundAngkasaPura.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.ItemClaim
{
    public class ItemClaimService : IItemClaimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IItemFoundService _itemFoundService;
        private readonly IMailerService _mailerService;
        private readonly UploadLocation _uploadLocation;
        private IMapper _mapper;

        public ItemClaimService(
            IUnitOfWork unitOfWork,
            IItemFoundService itemFoundService,
            IMailerService mailerService,
            UploadLocation uploadLocation)
        {
            _unitOfWork = unitOfWork;
            _itemFoundService = itemFoundService;
            _uploadLocation = uploadLocation;
            _mailerService = mailerService;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemClaim, ItemClaimResponseDTO>();
            }));
        }

        public async Task<ItemClaimResponseDTO> ClaimItem(ItemClaimRequestDTO request, string userId)
        {
            var itemClaim = new DAL.Model.ItemClaim()
            {
                ActiveFlag = true,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                ItemFoundId = request.ItemFoundId,
                ProofDescription = request.ProofDescription,
                IdentityNumber = request.IdentityNumber,
                IdentityType = request.IdentityType,
                Status = ItemFoundStatus.Confirmation,
                UserId = userId,
            };
            if (!String.IsNullOrWhiteSpace(request.ProofImageBase64))
            {
                var (extension, image) = Utils.GeneralUtils.GetDetailImageBase64(request.ProofImageBase64);
                var fileName = $"{userId}-{DateTime.Now.ToString("yyyy-MM-dd")}.{extension}";
                var fileLocation = _uploadLocation.ItemClaimLocation(fileName);
                Utils.GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(fileLocation));
                itemClaim.ProofImage = fileLocation;
            }
            else throw new DataMessageError(ErrorMessageConstant.ImageEmpty);
            var itemFound = await _unitOfWork.ItemFoundRepository.Where(t => t.Id.Equals(request.ItemFoundId)).FirstOrDefaultAsync();
            if (!itemFound.Status.ToLower().Equals(ItemFoundStatus.Found.ToLower()))
                throw new DataMessageError(ErrorMessageConstant.ItemInClaimProgress);
            await _itemFoundService.UpdateStatus(ItemFoundStatus.Confirmation, userId, itemFound);
            await _unitOfWork.ItemClaimRepository.AddAsync(itemClaim);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<ItemClaimResponseDTO>(itemClaim);
            result.ItemFoundId = itemFound.Id;
            result.Image = itemFound.Image;
            result.Name = itemFound.Name;
            result.Description = itemFound.Description;
            return result;
        }

        private void ApproveRejectConfirmation(DAL.Model.ItemClaim? itemClaim, DAL.Model.ItemFound itemFound)
        {
            if (itemClaim == null) throw new NotFoundError();
            if (!itemClaim.Status.ToLower().Equals(ItemFoundStatus.Confirmation.ToLower()))
                throw new DataMessageError(ErrorMessageConstant.ClaimApprovedRejectOnlyConfirmation);
            if (!itemFound.Status.ToLower().Equals(ItemFoundStatus.Confirmation.ToLower()))
                throw new DataMessageError(ErrorMessageConstant.ItemApprovedRejectOnlyConfirmation);
        }

        public async Task<ItemClaimResponseDTO> ApproveClaim(ItemClaimApproveRequestDTO request, string itemClaimId, string userId)
        {
            var itemClaim = await _unitOfWork.ItemClaimRepository
                                        .Include(t=>t.User)
                                        .Include(t=>t.ItemFound)
                                        .Where(t => t.Id.Equals(itemClaimId)).FirstOrDefaultAsync();
            if(itemClaim == null) throw new NotFoundError();
            var itemFound = itemClaim.ItemFound;
            ApproveRejectConfirmation(itemClaim, itemFound);
            await _itemFoundService.UpdateStatus(ItemFoundStatus.Confirmed, userId, itemFound);
            itemClaim.Status = ItemFoundStatus.Approved;
            itemClaim.LastUpdatedBy = userId;
            itemClaim.LastUpdatedDate = DateTime.Now;
            DAL.Model.ItemClaimApproval approval = new ItemClaimApproval()
            {
                ActiveFlag = true,
                Status = ItemFoundStatus.Rejected,
                ClaimDate = request.ClaimDate,
                ClaimLocation = request.ClaimLocation,
            };
            _unitOfWork.ItemClaimApprovalRepository.Add(approval);
            _unitOfWork.ItemClaimRepository.Update(itemClaim);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<ItemClaimResponseDTO>(itemClaim);
            result.ItemFoundId = itemFound.Id;
            result.Image = itemFound.Image;
            result.Name = itemFound.Name;
            result.Description = itemFound.Description;
            _mailerService.ApproveClaim(itemClaim.User.Email, request.ClaimLocation, request.ClaimDate, _uploadLocation.WebsiteUrl(itemClaimId));
            return result;
        }

        public async Task<Pagination<ItemClaimResponseDTO>> GetListItemClaim(int page, int size, bool isAdmin, string? userId)
        {
            var query = _unitOfWork.ItemClaimRepository
                                    .Include(t => t.ItemFound)
                                    .Where(t =>
                                    t.ActiveFlag &&
                                    (isAdmin || t.UserId.Equals(userId))
                                    );

            return await ConvertToResponse(query, page, size);
        }


        public async Task<Pagination<ItemClaimResponseDTO>> GetListItemClaimByItemFoundId(int page, int size, string itemFoundId)
        {
            var query = _unitOfWork.ItemClaimRepository
                                    .Include(t => t.ItemFound)
                                    .Where(t =>
                                    t.ActiveFlag &&
                                    t.ItemFoundId.Equals(itemFoundId)
                                    )
                                    .OrderByDescending(t =>t.CreatedDate);
            return await ConvertToResponse(query, page, size);
        }

        private async Task<Pagination<ItemClaimResponseDTO>> ConvertToResponse(IQueryable<DAL.Model.ItemClaim> query, int page, int size){

            var count = await query.CountAsync();
            var data = await query
                                .Skip((page - 1) * size)
                                .Take(size)
                                .Select(t => new ItemClaimResponseDTO()
                                {
                                    Id = t.Id,
                                    ProofDescription = t.ProofDescription,
                                    IdentityNumber = t.IdentityNumber,
                                    IdentityType = t.IdentityType,
                                    Description = t.ItemFound.Description,
                                    Image = _uploadLocation.Url(t.ItemFound.Image),
                                    Name = t.ItemFound.Name,
                                    ProofImage = _uploadLocation.Url(t.ProofImage),
                                    ItemFoundId = t.ItemFoundId,
                                    Status = t.Status
                                }).ToListAsync();
            return new Pagination<ItemClaimResponseDTO>(data, count, size, page);
        }

        public async Task<ItemClaimResponseDTO> RejectClaim(ItemClaimRejectRequestDTO request, string itemClaimId, string userId)
        {

            var itemClaim = await _unitOfWork.ItemClaimRepository
                .Include(t=>t.ItemFound)
                .Include(t=>t.User)
                .Where(t => t.Id.Equals(itemClaimId)).FirstOrDefaultAsync();
            if (itemClaim == null) throw new NotFoundError();
            var itemFound = itemClaim.ItemFound;
            ApproveRejectConfirmation(itemClaim, itemFound);
            await _itemFoundService.UpdateStatus(ItemFoundStatus.Found, userId, itemFound);
            itemClaim.Status = ItemFoundStatus.Rejected;
            itemClaim.LastUpdatedBy = userId;
            itemClaim.LastUpdatedDate = DateTime.Now;
            DAL.Model.ItemClaimApproval approval = new ItemClaimApproval()
            {
                ActiveFlag = true,
                Status = ItemFoundStatus.Rejected,
                RejectReason = request.RejectReason
            };
            _unitOfWork.ItemClaimApprovalRepository.Add(approval);
            _unitOfWork.ItemClaimRepository.Update(itemClaim);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<ItemClaimResponseDTO>(itemClaim);
            result.ItemFoundId = itemFound.Id;
            result.Image = itemFound.Image;
            result.Name = itemFound.Name;
            result.Description = itemFound.Description;
            _mailerService.RejectClaim(itemClaim.User.Email, request.RejectReason, _uploadLocation.WebsiteUrl(itemClaimId));
            return result;
        }

    }
}
