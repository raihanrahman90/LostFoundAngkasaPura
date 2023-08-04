
using LostFoundAngkasaPura.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace LostFoundAngkasaPura.DAL
{
    public class LostFoundDbContext : DbContext
    {
        public DbSet<Admin> admin { get; set; }
        public DbSet<ItemCategory> item_category { get; set; }
        public DbSet<ItemClaim> item_claim { get; set; }
        public DbSet<ItemClaimApproval> item_claim_approval { get; set; }
        public DbSet<ItemComment> item_comment { get; set; }
        public DbSet<ItemFound> item_found { get; set; }
        public DbSet<User> user { get; set; }
        public LostFoundDbContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Admin>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<ItemCategory>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<ItemClaim>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<ItemClaimApproval>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<ItemComment>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<ItemFound>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
            builder.Entity<User>(e => e.Property(e => e.Id).ValueGeneratedOnAdd());
        }
    }
}
