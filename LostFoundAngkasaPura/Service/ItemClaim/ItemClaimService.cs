using AutoMapper;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemFound;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.ItemClaim
{
    public class ItemClaimService : IItemClaimService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IItemFoundService _itemFoundService;
        private readonly UploadLocation _uploadLocation;
        private IMapper _mapper;

        public ItemClaimService(
            IUnitOfWork unitOfWork,
            IItemFoundService itemFoundService,
            UploadLocation uploadLocation)
        {
            _unitOfWork = unitOfWork;
            _itemFoundService = itemFoundService;
            _uploadLocation = uploadLocation;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemClaim, ItemClaimResponseDTO>();   
            }));
        }

        public async Task<ItemClaimResponseDTO> ClaimItem(ItemClaimRequestDTO request,  string userId)
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
            else
            {
                throw new DataMessageError(ErrorMessageConstant.ImageEmpty);
            }
            var itemFound = await _itemFoundService.UpdateStatus(ItemFoundStatus.Confirmation, request.ItemFoundId);
            if (!itemFound.Status.ToLower().Equals(ItemFoundStatus.Found.ToLower()))
                throw new DataMessageError(ErrorMessageConstant.ItemInClaimProgress);
            await _unitOfWork.ItemClaimRepository.AddAsync(itemClaim);
            await _unitOfWork.SaveAsync();
            var result = _mapper.Map<ItemClaimResponseDTO>(itemClaim);
            result.ItemFoundId = itemFound.Id;
            result.Image = itemFound.Image;
            result.Name = itemFound.Name;
            result.Description = itemFound.Description;
            return result;
        }

        public Task<ItemClaimResponseDTO> ConfirmClaim(ItemClaimRequestDTO request, string itemFoundId, string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<Pagination<ItemClaimResponseDTO>> GetListItemClaim(int page, int size, bool isAdmin, string? userId)
        {
            var query = _unitOfWork.ItemClaimRepository
                                    .Include(t => t.ItemFound)
                                    .Where(t =>
                                    t.ActiveFlag &&
                                    (isAdmin || t.UserId.Equals(userId))
                                    );
            var count = await query.CountAsync();
            var data = await query
                                .Skip((page - 1) * size)
                                .Take(size)
                                .Select(t => new ItemClaimResponseDTO()
                                {
                                    Id = t.Id,
                                    ProofDescription =t.ProofDescription,
                                    IdentityNumber = t.IdentityNumber,
                                    IdentityType = t.IdentityType,
                                    Description = t.ItemFound.Description,
                                    Image = _uploadLocation.Url(t.ItemFound.Image),
                                    Name = t.ItemFound.Name,
                                    ProofImage = _uploadLocation.Url(t.ProofImage),
                                    ItemFoundId = t.ItemFoundId
                                }).ToListAsync();
            return new Pagination<ItemClaimResponseDTO>(data, count, size, page);
        }

        public Task<ItemClaimResponseDTO> RejectClaim(ItemClaimRequestDTO request, string itemFoundId, string userId)
        {
            throw new NotImplementedException();
        }
    }
}
