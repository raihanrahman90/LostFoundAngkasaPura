using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO;
using LostFoundAngkasaPura.DTO.Error;
using LostFoundAngkasaPura.DTO.ItemFound;
using LostFoundAngkasaPura.Service.ItemCategory;
using LostFoundAngkasaPura.Service.Mailer;
using LostFoundAngkasaPura.Service.UserNotification;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Text.Json;
using static LostFoundAngkasaPura.Constant.Constant;

namespace LostFoundAngkasaPura.Service.ItemFound
{
    public class ItemFoundService : IItemFoundService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMailerService _mailerService;
        private readonly IItemCategoryService _itemCategoryService;
        private readonly UploadLocation _uploadLocation;
        private readonly IUserNotificationService _userNotificationService;
        private readonly LoggerUtils _logger;
        private IMapper _mapper;

        public ItemFoundService(IUnitOfWork unitOfWork, IMailerService mailerService, UploadLocation uploadLocation, IItemCategoryService itemCategoryService, LoggerUtils logger, IUserNotificationService userNotificationService)
        {
            _unitOfWork = unitOfWork;
            _mailerService = mailerService;
            _itemCategoryService = itemCategoryService;
            _uploadLocation = uploadLocation;
            _logger = logger;
            _userNotificationService = userNotificationService;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemFound, ItemFoundResponseDTO>()
                .ForMember(t => t.Image, t => t.MapFrom(d => _uploadLocation.Url(d.Image)))
                .ForMember(t => t.FoundDate, t => t.MapFrom(d => d.FoundDate.ToString("yyyy-MM-dd")))
                .ForMember(t => t.ClosingImage, t => t.MapFrom(d =>
                        d.ClosingDocumentation == null ? null :
                        _uploadLocation.Url(d.ClosingDocumentation.TakingItemImage)))
                .ForMember(t => t.ClosingDocumentation, t => t.MapFrom(d =>
                        d.ClosingDocumentation == null ? null :
                        _uploadLocation.Url(d.ClosingDocumentation.NewsDocumentation)))
                .ForMember(t => t.ClosingAgent, t => t.MapFrom(d =>
                        d.ClosingDocumentation == null ? null : d.ClosingDocumentation.ClosingAgent));
            }));
        }

        public async Task<ItemFoundResponseDTO> ClosedItem(ItemFoundClosingRequestDTO request, string itemFoundId, string userId)
        {
            _logger.LogInfo($"closing start itemFoundId:{itemFoundId}");
            var itemFound = await GetItemFoundById(itemFoundId);
            var itemClaimApproved = await _unitOfWork.ItemClaimRepository
                .Where(t => t.ItemFoundId.Equals(itemFoundId) && t.Status.Equals(ItemFoundStatus.Approved))
                .FirstOrDefaultAsync();
            ItemFoundResponseDTO response;
            //upload closing image
            
            var transaction = await _unitOfWork.CreateSavepoint($"closing:{itemFoundId}");
            try
            {
                await CreateDocumentClosing(request, itemFound, itemClaimApproved, userId);
                response = await UpdateStatus(ItemFoundStatus.Closed, userId, itemFound);
                var listItemClaim = await _unitOfWork.ItemClaimRepository
                    .Where(t => t.ItemFoundId.Equals(itemFoundId)).ToListAsync();
                await RejectAllItemClaim(listItemClaim);
                await DeleteAllNotification(listItemClaim);
                await transaction.CommitAsync();
            } 
            catch (DataMessageError e)
            {
                transaction.RollbackToSavepointAsync($"closing:{itemFoundId}");
                throw e;
            }
            catch (Exception e)
            {
                _logger.LogWarning(e.Message);
                transaction.RollbackToSavepointAsync($"closing:{itemFoundId}");
                throw new DataMessageError(ErrorMessageConstant.Unexpected);
            }

            //send notification
            if (itemClaimApproved != null)
            {
                await _userNotificationService.Closing(itemClaimApproved.UserId, itemClaimApproved.Id, itemFound.Name);
                _logger.LogInfo($"send notification rating for claim {itemClaimApproved.Id}");
            }

            return response;
        }

        private async Task<string> UploadImageClosing(string image64, string itemFoundId)
        {
            var (extension, image) = GeneralUtils.GetDetailImageBase64(image64);
            if (!ValidImageExtension.Contains(extension.ToLower()))
            {
                _logger.LogError($"image extension not valid {extension}");
                throw new DataMessageError(ErrorMessageConstant.ImageNotValid);
            }
            var pathImageClosing = _uploadLocation.ClosingLocation($"{itemFoundId}.{extension}"); 
            GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(pathImageClosing));
            return pathImageClosing;
        }

        private async Task<string> UploadDocumentClosing(string file64, string itemFoundId)
        {
            var (extension, file) = GeneralUtils.GetDetailDocument64(file64);
            if (!ValidDocumentExtension.Contains(extension.ToLower()))
            {
                _logger.LogError($"document extension not valid {extension}");
                throw new DataMessageError(ErrorMessageConstant.DocumentNotValid);
            }
            //extension perlu diformat ulang karena extension base64 dari docx adalah document bla bla bla
            extension = extension == "pdf" ? "pdf" : "docx";
            var pathDocumentClosing = _uploadLocation.ClosingLocation($"{itemFoundId}.{extension}");
            GeneralUtils.UploadFile(file, _uploadLocation.FolderLocation(pathDocumentClosing));
            return pathDocumentClosing;
        }

        private async Task RejectAllItemClaim(List<DAL.Model.ItemClaim> listItemClaim)
        {
            var updateStatus = listItemClaim.Where(t => !t.Status.Equals(ItemFoundStatus.Approved)).ToList();
            foreach (var update in updateStatus)
            {
                update.Status = ItemFoundStatus.Rejected;
            }
            _unitOfWork.ItemClaimRepository.UpdateRange(updateStatus);
            _logger.LogInfo($"reject claim id in {updateStatus.Select(t => t.Id).ToList()}");
        }

        private async Task DeleteAllNotification(List<DAL.Model.ItemClaim> listItemClaim)
        {
            var idDeleteNotif = listItemClaim.Select(t => t.Id).ToList();
            var deleteAdminNotif = await _unitOfWork.AdminNotificationRepository.Where(t => idDeleteNotif.Contains(t.ItemClaimId)).ToListAsync();
            var deleteUserNotif = await _unitOfWork.UserNotificationRepository.Where(t => idDeleteNotif.Contains(t.ItemClaimId)).ToListAsync();
            _unitOfWork.AdminNotificationRepository.RemoveRange(deleteAdminNotif);
            _unitOfWork.UserNotificationRepository.RemoveRange(deleteUserNotif);
            await _unitOfWork.SaveAsync();
            _logger.LogInfo($"delete all admin notification with id {idDeleteNotif}");
            _logger.LogInfo($"delete all user notification with id {idDeleteNotif}");

        }

        public async Task<ItemFoundResponseDTO> CreateItemFound(ItemFoundCreateRequestDTO request, string adminId)
        {
            _logger.LogInfo($"create item found: {request.Name}");
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
                _logger.LogInfo("upload image");
                var (extension, image) = GeneralUtils.GetDetailImageBase64(request.ImageBase64);
                if (!ValidImageExtension.Contains(extension.ToLower()))
                {
                    _logger.LogError("image not valid");
                    throw new DataMessageError(ErrorMessageConstant.ImageNotValid);
                }

                var fileName = $"{request.Name}-{DateTime.Now.ToString("yyyy-MM-dd")}.{extension.ToLower()}";
                var fileLocation = _uploadLocation.ItemFoundLocation(fileName);
                itemFound.Image = fileLocation;
                GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(fileLocation));
            }
            else
            {
                _logger.LogError("image is empty");
                throw new DataMessageError(ErrorMessageConstant.ImageEmpty);
            }
            await _unitOfWork.ItemFoundRepository.AddAsync(itemFound);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ItemFoundResponseDTO>(itemFound);
        }

        public async Task<ItemFoundResponseDTO> GetDetailItemFound(string itemFoundId)
        {
            var result = await _unitOfWork.ItemFoundRepository
                .Where(t => t.Id.Equals(itemFoundId))
                .Include(t => t.ClosingDocumentation)
                .Select(t => _mapper.Map<ItemFoundResponseDTO>(t))
                .FirstOrDefaultAsync();
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

        public async Task<ItemFoundResponseDTO> UpdateStatus(
            string status,string userId, 
            DAL.Model.ItemFound itemFound)
        {
            itemFound.Status = status;
            return await UpdateItemFound(itemFound, userId);
        }

        public async Task<ItemFoundResponseDTO> UpdateItemFound(DAL.Model.ItemFound itemFound, string userId)
        {
            itemFound.LastUpdatedDate = DateTime.Now;
            itemFound.LastUpdatedBy = userId;
            _unitOfWork.ItemFoundRepository.Update(itemFound);
            await _unitOfWork.SaveAsync();
            return _mapper.Map<ItemFoundResponseDTO>(itemFound);
        }
        private async Task CreateDocumentClosing(
            ItemFoundClosingRequestDTO request, 
            DAL.Model.ItemFound itemFound, 
            DAL.Model.ItemClaim itemClaimApproved,
            string userId)
        {
            var pathImage = await UploadImageClosing(request.Image, itemFound.Id);
            var pathDocument = await UploadDocumentClosing(request.News, itemFound.Id);
            ClosingDocumentation closingDocument = new ClosingDocumentation()
            {
                ActiveFlag = true,
                CreatedBy = userId,
                CreatedDate = DateTime.Now,
                TakingItemImage = pathImage,
                NewsDocumentation = pathDocument,
                ClosingAgent = request.Agent,
                ItemFound = itemFound,
                ItemClaim = itemClaimApproved
            };
            await _unitOfWork.ClosingDocumentationRepository.AddAsync(closingDocument);
            await _unitOfWork.SaveAsync();

        }

        private async Task<DAL.Model.ItemFound> GetItemFoundById(string itemFoundId)
        {
            var itemFound = await _unitOfWork.ItemFoundRepository
            .Where(t => t.Id.Equals(itemFoundId))
            .FirstOrDefaultAsync();
            if (itemFound == null)
            {
                _logger.LogError($"data not found itemFoundId:{itemFoundId}");
                throw new NotFoundError();
            }
            return itemFound;

        }
    }
}
