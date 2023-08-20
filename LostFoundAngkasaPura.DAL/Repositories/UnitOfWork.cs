using LostFoundAngkasaPura.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LostFoundAngkasaPura.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext dbContext;

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

        public UnitOfWork(LostFoundDbContext dbContext, IConfiguration configuration)
        {
            this.AdminRepository = dbContext.admin;
            this.AdminNotificationRepository = dbContext.admin_notification;
            this.ItemCategoryRepository = dbContext.item_category;
            this.ItemClaimRepository = dbContext.item_claim;
            this.ItemClaimApprovalRepository = dbContext.item_claim_approval;
            this.ItemCommentRepository = dbContext.item_comment;
            this.ItemFoundRepository = dbContext.item_found;
            this.UserRepository = dbContext.user;
            this.UserForgotPasswordRepository = dbContext.user_forgot_password;
            this.UserNotificationRepository = dbContext.user_notification;
            this.dbContext = dbContext;
        }

        public async Task SaveAsync()
        {
            await this.dbContext.SaveChangesAsync();
        }
        public void Save()
        {
            this.dbContext.SaveChanges();
        }
    }
}
