using LostFound.DAL.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace LostFound.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private DbContext dbContext;

        public DbSet<Admin> AdminRepository { get; set; }
        public DbSet<User> UserRepository { get; set; }

        public UnitOfWork(LostFoundDbContext dbContext, IConfiguration configuration)
        {
            this.AdminRepository = dbContext.admin;
            this.UserRepository = dbContext.user;
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
