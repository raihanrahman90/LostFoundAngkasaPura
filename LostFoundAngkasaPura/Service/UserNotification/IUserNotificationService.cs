using LostFoundAngkasaPura.DTO.Notification;

namespace LostFoundAngkasaPura.Service.UserNotification
{
    public interface IUserNotificationService
    {
        Task<List<NotificationResponse>> GetListNotification(string userId);
        Task<int> CountNotification(string userId);
        Task NewComment(string adminId, string itemClaimId, string itemName);
        Task Approve(string userId, string itemClaimId, string itemName);
        Task Reject(string userId, string itemClaimId, string itemName);
        Task Closing(string userId, string itemClaimId, string itemName);
        Task DeleteNotification(string userId, string itemClaimId);
    }
}
