using LostFoundAngkasaPura.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.DAL.Repositories
{
    public interface IUnitOfWork
    {
        public DbSet<Admin> AdminRepository { get; set; }
        public DbSet<AdminNotification> AdminNotificationRepository { get; set; }
        public DbSet<ItemCategory> ItemCategoryRepository { get; set; }
        public DbSet<ItemClaim> ItemClaimRepository { get; set; }
        public DbSet<ItemClaimApproval> ItemClaimApprovalRepository { get; set; }
        public DbSet<ItemComment> ItemCommentRepository { get; set; }
        public DbSet<ItemFound> ItemFoundRepository { get; set; }
        public DbSet<User> UserRepository { get; set; }
        public DbSet<UserForgotPassword> UserForgotPasswordRepository { get; set; }
        public DbSet<UserNotification> UserNotificationRepository { get; set; }
        Task SaveAsync();
        void Save();
    }
}
