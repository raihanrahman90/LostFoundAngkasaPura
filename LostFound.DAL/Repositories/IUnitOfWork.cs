using LostFound.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace LostFound.DAL.Repositories
{
    public interface IUnitOfWork
    {
        public DbSet<Admin> AdminRepository { get; set; }
        public DbSet<User> UserRepository { get; set; }
        Task SaveAsync();
        void Save();
    }
}
