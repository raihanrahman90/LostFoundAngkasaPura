using AutoMapper;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.DTO.ItemClaim;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.AdminNotification;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Service.Mailer;
using LostFoundAngkasaPura.Service.UserNotification;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.ItemFound
{
    public class ItemFoundService : IItemFoundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailerService _mailerService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly UploadLocation _uploadLocation;
        private IMapper _mapper;

        public ItemFoundService(IUnitOfWork unitOfWork, IMailerService mailerService, UploadLocation uploadLocation, IItemCategoryService itemCategoryService)
        {
            _unitOfWork = unitOfWork;
            _mailerService = mailerService;
            _itemCategoryService = itemCategoryService;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemFound, ItemFoundResponseDTO>()
                .ForMember(t=>t.Image, t=>t.MapFrom(d=>_uploadLocation.Url(d.Image)))
                .ForMember(t=>t.FoundDate, t=>t.MapFrom(d=>d.FoundDate.ToString("yyyy-MM-dd")));
            }));
            _uploadLocation = uploadLocation;
        }

        public async Task<ItemFoundResponseDTO> ClosedItem(string itemFoundId, string userId)
        {
            var itemFound = await _unitOfWork.ItemFoundRepository.Where(t=>t.Id.Equals(itemFoundId)).FirstOrDefaultAsync();
            if (itemFound == null) throw new NotFoundError();
            var response = await UpdateStatus(ItemFoundStatus.Closed, userId, itemFound);
            var listItemClaim = await _unitOfWork.ItemClaimRepository.Where(t => t.ItemFoundId.Equals(itemFound)).ToListAsync();

            //update not approved status
            var updateStatus = listItemClaim.Where(t=>!t.Status.Equals(ItemFoundStatus.Approved)).ToList();
            foreach(var update in updateStatus)
            {
                update.Status = ItemFoundStatus.Rejected;
            }
            _unitOfWork.ItemClaimRepository.UpdateRange(updateStatus);

            //delete notification
            var idDeleteNotif = listItemClaim.Select(t => t.Id).ToList();
            var deleteAdminNotif = await _unitOfWork.AdminNotificationRepository.Where(t => idDeleteNotif.Contains(t.ItemClaimId)).ToListAsync();
            var deleteUserNotif = await _unitOfWork.UserNotificationRepository.Where(t => idDeleteNotif.Contains(t.ItemClaimId)).ToListAsync();
            _unitOfWork.AdminNotificationRepository.RemoveRange(deleteAdminNotif);
            _unitOfWork.UserNotificationRepository.RemoveRange(deleteUserNotif);
            await _unitOfWork.SaveAsync();

            return response;
        }

        public async Task<ItemFoundResponseDTO> CreateItemFound(ItemFoundCreateRequestDTO request, string adminId)
        {
            var isUploadImage = String.IsNullOrWhiteSpace(request.ImageBase64);
            var category = await _itemCategoryService.CreateCategory(request.Category);
            DAL.Model.ItemFound itemFound = new DAL.Model.ItemFound(){
                Name = request.Name,
                Description = request.Description,
                Category = category.Category,
                ActiveFlag = true,
                AdminId = adminId,
                FoundDate = request.FoundDate,
                Status = ItemFoundStatus.Found
            };
            if (!isUploadImage)
            {
                var (extension, image) = Utils.GeneralUtils.GetDetailImageBase64(request.ImageBase64);
                if (!Constant.Constant.ValidImageExtension.Contains(extension.ToLower()))
                    throw new DataMessageError(ErrorMessageConstant.ImageNotValid);

                var fileName = $"{request.Name}-{DateTime.Now.ToString("yyyy-MM-dd")}.{extension.ToLower()}";
                var fileLocation = _uploadLocation.ItemFoundLocation(fileName);
                itemFound.Image = fileLocation;
                GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(fileLocation));
            }
            else
            {
                throw new DataMessageError(ErrorMessageConstant.ImageEmpty);
            }
            await _unitOfWork.ItemFoundRepository.AddAsync(itemFound);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ItemFoundResponseDTO>(itemFound);
        }

        public async Task<ItemFoundResponseDTO> GetDetailItemFound(string itemFoundId)
        {
            var result = await _unitOfWork.ItemFoundRepository.Where(t => t.Id.Equals(itemFoundId)).Select(t => _mapper.Map<ItemFoundResponseDTO>(t)).FirstOrDefaultAsync();
            if (result == null) throw new NotFoundError();
            return result;
        }

        public async Task<Pagination<ItemFoundResponseDTO>> GetListItemFound(int page, int size, string? name, string? category, string? status, DateTime? foundDateStart, DateTime? foundDateEnd)
        {
            var query = _unitOfWork.ItemFoundRepository
                                    .Where(t =>
                                    t.ActiveFlag &&
                                    (name == null || t.Name.ToLower().Contains(name.ToLower())) &&
                                    (category == null || t.Category.ToLower().Equals(category.ToLower())) &&
                                    (foundDateStart == null || t.FoundDate >= foundDateStart) &&
                                    (foundDateEnd == null || t.FoundDate <= foundDateEnd) &&
                                    (status == null || t.Status.ToLower().Equals(status.ToLower())));

            var count = await query.CountAsync();
            var data = await query
                                .Skip((page - 1) * size)
                                .Take(size)
                                .OrderByDescending(t=>t.CreatedDate)
                                .Select(t=>_mapper.Map<ItemFoundResponseDTO>(t))
                                .ToListAsync();
            return new Pagination<ItemFoundResponseDTO>(data, count, size, page);
        }

        public Task<ItemFoundResponseDTO> UpdateItemFound(ItemFoundCreateRequestDTO request, string itemFoundId, string adminId)
        {
            throw new NotImplementedException();
        }

        public async Task<ItemFoundResponseDTO> UpdateStatus(string status, string userId, DAL.Model.ItemFound itemFound)
        {
            itemFound.Status = status;
            itemFound.LastUpdatedDate = DateTime.Now;
            itemFound.LastUpdatedBy = userId;
            _unitOfWork.ItemFoundRepository.Update(itemFound);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ItemFoundResponseDTO>(itemFound);
        }
    }
}
