using LostFoundAngkasaPura.DTO.Notification;

namespace LostFoundAngkasaPura.Service.AdminNotification
{
    public interface IAdminNotificationService
    {
        Task<List<NotificationResponse>> GetListNotification(string adminId);
        Task<int> CountNotification(string adminId);
        Task NewComment(string adminId, string itemClaimId, string itemName);

        Task NewClaim(string adminId, string itemClaimId, string itemName);

        Task DeleteNotification(string adminId, string itemClaimId);
    }
}
