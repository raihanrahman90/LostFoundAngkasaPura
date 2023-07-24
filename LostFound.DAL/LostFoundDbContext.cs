
using LostFound.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace LostFound.DAL
{
    public class LostFoundDbContext : DbContext
    {
        public DbSet<Admin> admin { get; set; }
        public DbSet<User> user { get; set; }
        public LostFoundDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Admin>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<User>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        }
    }
}
