using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.Notification;
using LostFoundAngkasaPura.Utils;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.UserNotification
{
    public class UserNotificationService : IUserNotificationService
    {
        private IUnitOfWork _unitOfWork;
        private LoggerUtils _logger;

        public UserNotificationService(IUnitOfWork uow, LoggerUtils logger)
        {
            _unitOfWork = uow;
            _logger = logger;
        }

        public async Task DeleteNotification(string userId, string itemClaimId)
        {
            _logger.LogInfo($"delete notiication of user:{userId} itemClaim:{itemClaimId}");
            var deleteNotification = await _unitOfWork.UserNotificationRepository.Where(t => t.UserId.Equals(userId) && t.ItemClaimId.Equals(itemClaimId)).ToListAsync();
            _unitOfWork.UserNotificationRepository.RemoveRange(deleteNotification);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<NotificationResponse>> GetListNotification(string userId)
        {
            var data = await _unitOfWork.UserNotificationRepository.Where(t => t.UserId.Equals(userId)).
                Select(t => new NotificationResponse()
                {
                    Title = t.Title,
                    Subtitle = t.Subtitle,
                    Url = t.Url
                }).ToListAsync();
            return data;
        }

        public async Task<int> CountNotification(string userId)
        {
            var count = await _unitOfWork.UserNotificationRepository.Where(t => t.UserId.Equals(userId)).CountAsync();
            return count;
        }

        public async Task NewComment(string userId, string itemClaimId, string itemName)
        {
            await DeleteNotification(userId, itemClaimId);
            var title = "Admin menambahkan komentar baru";
            var subtitle = $"Terdapat komentar baru terhadap claim pada '{itemName}'";
            await addNotification(itemClaimId, userId, title, subtitle);
        }

        public async Task Approve(string userId, string itemClaimId, string itemName)
        {
            await DeleteNotification(userId, itemClaimId);
            var title = "Admin telah menyetujui klaim Anda";
            var subtitle = $"Lihat detail pengambilan pada halaman claim";
            await addNotification(itemClaimId, userId, title, subtitle);
        }

        public async Task Reject(string userId, string itemClaimId, string itemName)
        {
            await DeleteNotification(userId, itemClaimId);
            var title = "Admin telah menolak klaim Anda";
            var subtitle = $"Lihat detail penolakan pada halaman claim";
            await addNotification(itemClaimId, userId, title, subtitle);
        }

        public async Task Closing(string userId, string itemClaimId, string itemName)
        {
            await DeleteNotification(userId, itemClaimId);
            var title = "Admin telah melakukan closing pada claim Anda";
            var subtitle = $"Berikan rating pada pelayan Lost & Found";
            await addNotification(itemClaimId, userId, title, subtitle);
        }

        private async Task addNotification(string itemClaimId,string userId, string title, string subtitle)
        {
            _logger.LogInfo($"add notification for user {userId} with itemClaimId {itemClaimId}");
            var notification = new DAL.Model.UserNotification()
            {
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                ActiveFlag = true,
                UserId = userId,
                ItemClaimId = itemClaimId,
                Title = title,
                Subtitle = subtitle,
                Url = $"/Claim/{itemClaimId}"
            };
            await _unitOfWork.UserNotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveAsync();
        }
    }
}
