using LostFoundAngkasaPura.DAL.Repositories;
using LostFoundAngkasaPura.DTO.Notification;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.Service.AdminNotification
{
    public class AdminNotificationService : IAdminNotificationService
    {
        private IUnitOfWork _unitOfWork;

        public AdminNotificationService(IUnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        public async Task DeleteNotification(string adminId, string itemClaimId)
        {
            var deleteNotification = await _unitOfWork.AdminNotificationRepository.Where(t => t.AdminId.Equals(adminId) && t.ItemClaimId.Equals(itemClaimId)).ToListAsync();
            _unitOfWork.AdminNotificationRepository.RemoveRange(deleteNotification);
            await _unitOfWork.SaveAsync();
        }

        public async Task<List<NotificationResponse>> GetListNotification(string adminId)
        {
            var data = await _unitOfWork.AdminNotificationRepository.Where(t => t.AdminId.Equals(adminId)).
                Select(t => new NotificationResponse()
                {
                    Title = t.Title,
                    Subtitle = t.Subtitle,
                    Url = t.Url
                }).ToListAsync();
            return data;
        }

        public async Task<int> CountNotification(string adminId)
        {
            var count = await _unitOfWork.AdminNotificationRepository.Where(t => t.AdminId.Equals(adminId)).CountAsync();
            return count;
        }

        public async Task NewClaim(string adminId, string itemClaimId, string itemName)
        {
            await DeleteNotification(adminId, itemClaimId);
            var title = "User melakukan Claim";
            var subtitle = $"Terdapat Claim baru terhadap barang '{itemName}'";
            var notification = new DAL.Model.AdminNotification()
            {
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                ActiveFlag = true,
                AdminId = adminId,
                ItemClaimId = itemClaimId,
                Title = title,
                Subtitle = subtitle,
                Url = $"/Admin/ItemClaim/{itemClaimId}"
            };
            await _unitOfWork.AdminNotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveAsync();
        }

        public async Task NewComment(string adminId, string itemClaimId, string itemName)
        {
            await DeleteNotification(adminId, itemClaimId);
            var title = "User menambahkan komentar baru";
            var subtitle = $"Terdapat komentar baru terhadap claim pada '{itemName}'";
            var notification = new DAL.Model.AdminNotification()
            {
                CreatedBy = "System",
                CreatedDate = DateTime.Now,
                ActiveFlag = true,
                AdminId = adminId,
                ItemClaimId = itemClaimId,
                Title = title,
                Subtitle = subtitle,
                Url = $"/Admin/ItemClaim/{itemClaimId}"
            };
            await _unitOfWork.AdminNotificationRepository.AddAsync(notification);
            await _unitOfWork.SaveAsync();
        }
    }
}
