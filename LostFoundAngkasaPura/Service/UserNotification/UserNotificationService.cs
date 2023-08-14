using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.Notification;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.UserNotification
{
    public class UserNotificationService : IUserNotificationService
    {
        private IUnitOfWork _unitOfWork;

        public UserNotificationService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task DeleteNotification(string userId, string itemClaimId)
        {
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

        public async Task Approve(string userId, string itemClaimId, string itemName)
        {
            await DeleteNotification(userId, itemClaimId);
            var title = "Admin telah menyetujui klaim Anda";
            var subtitle = $"Lihat detail pengambilan pada halaman claim";
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

        public async Task Reject(string userId, string itemClaimId, string itemName)
        {
            await DeleteNotification(userId, itemClaimId);
            var title = "Admin telah menolak klaim Anda";
            var subtitle = $"Lihat detail penolakan pada halaman claim";
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
