using Microsoft.EntityFrameworkCore;
using Dashboards.Server.Models.Entities;

namespace Dashboards.Server.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserInformation> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Market> Markets { get; set; }
        public DbSet<RoleMarket> RoleMarkets { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<UserTypePage> UserTypePages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserInformation>(entity =>
            {
                entity.ToTable("Information", "Users");
                entity.HasKey(e => e.UserId);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(e => e.RoleId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(e => e.UserType)
                    .WithMany(ut => ut.UsersInformations)
                    .HasForeignKey(e => e.UserTypeId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.ToTable("Roles", "Type");
                entity.HasKey(e => e.RoleId);

                entity.HasOne(e => e.UserType)
                    .WithMany()
                    .HasForeignKey(e => e.UserTypeId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<UserType>(entity =>
            {
                entity.ToTable("Users", "Type");
                entity.HasKey(e => e.UserTypeId);
            });

            modelBuilder.Entity<Market>(entity =>
            {
                entity.ToTable("Market", "Type");
                entity.HasKey(e => e.MarketId);
            });

            modelBuilder.Entity<RoleMarket>(entity =>
            {
                entity.ToTable("RoleMarkets", "Users");
                entity.HasKey(e => e.RoleMarketId);

                entity.HasOne(e => e.Role)
                    .WithMany(r => r.RoleMarkets)
                    .HasForeignKey(e => e.RoleId);

                entity.HasOne(e => e.Market)
                    .WithMany(m => m.RoleMarkets)
                    .HasForeignKey(e => e.MarketId);
            });

            modelBuilder.Entity<Page>(entity =>
            {
                entity.ToTable("Pages", "Security");
                entity.HasKey(e => e.PageId);
            });

            modelBuilder.Entity<UserTypePage>(entity =>
            {
                entity.ToTable("UserTypePages", "Security");
                entity.HasKey(e => new { e.UserTypeId, e.PageId });

                entity.Property(e => e.UserTypePageId)
                    .ValueGeneratedOnAdd();

                entity.HasOne(e => e.Page)
                    .WithMany(p => p.UserTypePages)
                    .HasForeignKey(e => e.PageId);

                entity.HasOne(e => e.UserType)
                    .WithMany(ut => ut.UserTypePages)
                    .HasForeignKey(e => e.UserTypeId);
            });
        }
    }
}
