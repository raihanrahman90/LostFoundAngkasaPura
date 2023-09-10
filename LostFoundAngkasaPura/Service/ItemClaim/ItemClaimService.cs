using AutoMapper;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.Service.AdminNotification;
using LostFoundAngkasaPura.Service.ItemFound;
using LostFoundAngkasaPura.Service.Mailer;
using LostFoundAngkasaPura.Service.UserNotification;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.ItemClaim
{
    public class ItemClaimService : IItemClaimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IItemFoundService _itemFoundService;
        private readonly IMailerService _mailerService;
        private readonly IAdminNotificationService _adminNotificationService;
        private readonly IUserNotificationService _userNotificationService;
        private readonly UploadLocation _uploadLocation;
        private IMapper _mapper;

        public ItemClaimService(
            IUnitOfWork unitOfWork,
            IItemFoundService itemFoundService,
            IMailerService mailerService,
            IAdminNotificationService adminNotificationService,
            IUserNotificationService userNotificationService,
            UploadLocation uploadLocation)
        {
            _unitOfWork = unitOfWork;
            _itemFoundService = itemFoundService;
            _uploadLocation = uploadLocation;
            _mailerService = mailerService;
            _adminNotificationService = adminNotificationService;
            _userNotificationService = userNotificationService;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemClaim, ItemClaimResponseDTO>()
                .ForMember(t => t.Description, t => t.MapFrom(t => t.ItemFound.Description))
                .ForMember(t => t.Image, t => t.MapFrom(d => _uploadLocation.Url(d.ItemFound.Image)))
                .ForMember(t => t.Name, t => t.MapFrom(d => d.ItemFound.Name))
                .ForMember(t => t.ProofImage, t => t.MapFrom(d => _uploadLocation.Url(d.ProofImage)))
                .ForMember(t => t.UserName, t => t.MapFrom(d => d.User.Name))
                .ForMember(t => t.UserPhoneNumber, t => t.MapFrom(d => d.User.Phone))
                .ForMember(t => t.UserEmail, t => t.MapFrom(d => d.User.Email))
                .ForMember(t => t.ItemFoundStatus, t => t.MapFrom(d => d.ItemFound.Status))
                .ForMember(t => t.ClaimDate, t => t.MapFrom(d =>
                            d.ItemClaimApproval.Where(t => t.Status.Equals(ItemFoundStatus.Approved)).FirstOrDefault() == null ?
                            null :
                            d.ItemClaimApproval.Where(t => t.Status.Equals(ItemFoundStatus.Approved)).First().ClaimDate.Value.ToString("yyyy-MM-dd")
                        ))
                .ForMember(t => t.ClaimLocation, t => t.MapFrom(d => d.ItemClaimApproval.Where(t => t.Status.Equals(ItemFoundStatus.Approved)).FirstOrDefault().ClaimLocation))
                .ForMember(t => t.RejectReason, t => t.MapFrom(d => d.ItemClaimApproval.Where(t => t.Status.Equals(ItemFoundStatus.Rejected)).FirstOrDefault().RejectReason))
                .ForMember(t => t.ApprovalBy, t => t.MapFrom(d => d.ItemClaimApproval.LastOrDefault().Admin.Name));
            })
            {

            });
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
                GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(fileLocation));
                itemClaim.ProofImage = fileLocation;
            }
            else throw new DataMessageError(ErrorMessageConstant.ImageEmpty);
            var itemFound = await _unitOfWork.ItemFoundRepository.Include(t=>t.Admin).Where(t => t.Id.Equals(request.ItemFoundId)).FirstOrDefaultAsync();
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
            await _adminNotificationService.NewClaim(itemFound.AdminId, result.Id, itemFound.Name);
            await _mailerService.CreateClaim(itemFound.Admin.Email, result.Id);
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
                                        .Include(t => t.ItemClaimApproval)
                                        .Where(t => t.Id.Equals(itemClaimId)).FirstOrDefaultAsync();
            if(itemClaim == null) throw new NotFoundError();
            var itemFound = itemClaim.ItemFound;
            ApproveRejectConfirmation(itemClaim, itemFound);
            await _itemFoundService.UpdateStatus(ItemFoundStatus.Confirmed, userId, itemFound);
            itemClaim.Status = ItemFoundStatus.Approved;
            itemClaim.LastUpdatedBy = userId;
            itemClaim.LastUpdatedDate = DateTime.Now;
            ItemClaimApproval approval = new ItemClaimApproval()
            {
                ActiveFlag = true,
                Status = ItemFoundStatus.Approved,
                ClaimDate = request.ClaimDate,
                ClaimLocation = request.ClaimLocation,
                ItemClaimId = itemClaimId,
                AdminId = userId
            };
            _unitOfWork.ItemClaimApprovalRepository.Add(approval);
            _unitOfWork.ItemClaimRepository.Update(itemClaim);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<ItemClaimResponseDTO>(itemClaim);
            result.ItemFoundId = itemFound.Id;
            result.Image = itemFound.Image;
            result.Name = itemFound.Name;
            result.Description = itemFound.Description;
            await _mailerService.ApproveClaim(itemClaim.User.Email, request.ClaimLocation, request.ClaimDate, _uploadLocation.WebsiteUrl(itemClaimId));
            await _userNotificationService.Approve(itemClaim.UserId, itemClaimId, itemFound.Name);
            await _adminNotificationService.DeleteNotification(userId, itemClaimId);
            return result;
        }

        public async Task<Pagination<ItemClaimResponseDTO>> GetListItemClaim(int page, int size, bool isAdmin, string? userId)
        {
            var query = _unitOfWork.ItemClaimRepository
                                    .Include(t => t.ItemFound)
                                    .Include(t => t.User)
                                    .Include(t => t.ItemClaimApproval)
                                    .Where(t =>
                                    t.ActiveFlag &&
                                    (isAdmin || t.UserId.Equals(userId))
                                    )
                                    .OrderByDescending(t=>t.CreatedDate);

            return await ConvertToResponse(query, page, size);
        }


        public async Task<Pagination<ItemClaimResponseDTO>> GetListItemClaimByItemFoundId(int page, int size, string itemFoundId, string? status)
        {
            var query = _unitOfWork.ItemClaimRepository
                                    .Include(t => t.ItemFound)
                                    .Include(t=>t.User)
                                    .Include(t => t.ItemClaimApproval)
                                    .Where(t =>
                                    t.ActiveFlag &&
                                    (itemFoundId == null || t.ItemFoundId.Equals(itemFoundId)) &&
                                    (status == null || t.Status.Equals(status))
                                    )
                                    .OrderByDescending(t =>t.CreatedDate);
            return await ConvertToResponse(query, page, size);
        }

        private async Task<Pagination<ItemClaimResponseDTO>> ConvertToResponse(IQueryable<DAL.Model.ItemClaim> query, int page, int size){

            var count = await query.CountAsync();
            var data = await query
                                .Skip((page - 1) * size)
                                .Take(size)
                                .Select(t => _mapper.Map<ItemClaimResponseDTO>(t)).ToListAsync();
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
            ItemClaimApproval approval = new ItemClaimApproval()
            {
                ActiveFlag = true,
                Status = ItemFoundStatus.Rejected,
                RejectReason = request.RejectReason,
                ItemClaimId = itemClaimId
            };
            _unitOfWork.ItemClaimApprovalRepository.Add(approval);
            _unitOfWork.ItemClaimRepository.Update(itemClaim);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<ItemClaimResponseDTO>(itemClaim);
            result.ItemFoundId = itemFound.Id;
            result.Image = itemFound.Image;
            result.Name = itemFound.Name;
            result.Description = itemFound.Description;
            await _mailerService.RejectClaim(itemClaim.User.Email, request.RejectReason, _uploadLocation.WebsiteUrl(itemClaimId));
            await _userNotificationService.Reject(itemClaim.UserId, itemClaimId, itemFound.Name);
            await _adminNotificationService.DeleteNotification(userId, itemClaimId);
            return result;
        }

        public async Task<ItemClaimResponseDTO> GetItemClaimDetail(string itemClaimId)
        {
            var result = await _unitOfWork.ItemClaimRepository
                                     .Include(t=>t.User)
                                     .Include(t=>t.ItemFound)
                                     .Include(t=>t.ItemClaimApproval)
                                    .Where(t=>t.Id.Equals(itemClaimId) && t.ActiveFlag)
                                    .Select(t => _mapper.Map<ItemClaimResponseDTO>(t)).FirstOrDefaultAsync();
            if (result == null) throw new NotFoundError();
            return result;
        }

        public async Task ValidateUser(string itemClaimId, string userId)
        {
            var claimerId = await _unitOfWork.ItemClaimRepository.Where(t => t.Id.Equals(itemClaimId)).Select(t=>t.UserId).FirstOrDefaultAsync();
            if (!claimerId.Equals(userId))
                throw new NotAuthorizeError();
        }
    }
}
