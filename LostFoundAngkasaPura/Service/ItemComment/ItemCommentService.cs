using AutoMapper;
using LostFoundAngkasaPura.DAL.Model;
using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.ItemComment;
using LostFoundAngkasaPura.Service.AdminNotification;
using LostFoundAngkasaPura.Service.UserNotification;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.ItemComment
{
    public class ItemCommentService : IItemCommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UploadLocation _uploadLocation;
        private readonly IUserNotificationService _userNotificationService;
        private readonly IAdminNotificationService _adminNotificationService;
        private IMapper _mapper;

        public ItemCommentService(
            IUnitOfWork uow, 
            UploadLocation uploadLocation,
            IUserNotificationService userNotificationService,
            IAdminNotificationService adminNotificationService)
        {
            _unitOfWork = uow;    
            _uploadLocation = uploadLocation;
            _userNotificationService = userNotificationService;
            _adminNotificationService = adminNotificationService;
            _mapper = new Mapper(new MapperConfiguration(t =>
            {
                t.CreateMap<DAL.Model.ItemComment, ItemCommentResponseDTO>()
                .ForMember(t=>t.Image, t=>t.MapFrom(t=>String.IsNullOrEmpty(t.ImageLocation)?null:_uploadLocation.Url(t.ImageLocation)))
                .ForMember(t=>t.UserName, t => t.MapFrom(t => t.UserId==null?t.Admin.Name:t.User.Name))
                .ForMember(t=>t.UserStatus, t=>t.MapFrom(t => t.UserId == null?"Admin":"User"));
                
            }));
        }

        public async Task<ItemCommentResponseDTO> AddComment(ItemCommentCreateRequestDTO request, string? userId, string? adminId)
        {
            var comment = new DAL.Model.ItemComment()
            {
                UserId = userId,
                AdminId = adminId,
                Value = request.Value,
                CreatedBy = userId??adminId,
                ItemClaimId = request.ItemClaimId,
            };
            if (!String.IsNullOrWhiteSpace(request.ImageBase64))
            {
                var (extension, image) = Utils.GeneralUtils.GetDetailImageBase64(request.ImageBase64);
                var fileName = $"{request.ItemClaimId}-{DateTime.Now.ToString("yyyy-MM-dd")}.{extension}";
                var fileLocation = _uploadLocation.ComentarLocation(fileName);
                Utils.GeneralUtils.UploadFile(image, _uploadLocation.FolderLocation(fileLocation));
               comment.ImageLocation = fileLocation;
            }
            await _unitOfWork.ItemCommentRepository.AddAsync(comment);
            await _unitOfWork.SaveAsync();
            if(userId != null)
            {
                //komen oleh user
                var adminCommentId = "";
                var lastComment = await _unitOfWork.ItemCommentRepository.Where(t => t.AdminId != null && t.ActiveFlag).OrderBy(t => t.CreatedDate).LastOrDefaultAsync();

                var itemFound = await _unitOfWork.ItemClaimRepository
                    .Include(t => t.ItemFound)
                    .Where(t => t.Id.Equals(request.ItemClaimId))
                    .Select(t => t.ItemFound)
                    .FirstOrDefaultAsync();
                if (lastComment == null) adminCommentId = itemFound.AdminId;
                else  adminCommentId = lastComment.AdminId;
                await _userNotificationService.DeleteNotification(userId, request.ItemClaimId);
                await _adminNotificationService.NewComment(adminCommentId, request.ItemClaimId, itemFound.Name);
            }
            else
            {
                //admin submit komen
                var itemClaim = await _unitOfWork.ItemClaimRepository
                    .Include(t => t.ItemFound)
                    .Where(t => t.Id.Equals(request.ItemClaimId))
                    .FirstOrDefaultAsync();
                await _adminNotificationService.DeleteNotification(adminId, request.ItemClaimId);
                await _userNotificationService.NewComment(itemClaim.UserId, request.ItemClaimId, itemClaim.ItemFound.Name);
            }
            return _mapper.Map<ItemCommentResponseDTO>(comment);
        }

        public async Task<List<ItemCommentResponseDTO>> GetComment(string itemClaimId)
        {
            var result = await _unitOfWork.ItemCommentRepository
                .Include(t=>t.User)
                .Include(t=>t.Admin)
                .Where(t => t.ItemClaimId.Equals(itemClaimId) && t.ActiveFlag)
                .OrderBy(t=>t.CreatedDate)
                .Select(t => _mapper.Map<ItemCommentResponseDTO>(t)).ToListAsync();
            return result;
        }
    }
}
