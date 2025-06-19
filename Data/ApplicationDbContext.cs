using BookShoppingCartMVC.Models;
using BookShoppingCartMvcUI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookShoppingCartMVC.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Application Tables
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Book> Books { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<CartDetail> CartDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<Stock> Stocks { get; set; }

        // ⛏ If you’re accessing "orderStatuses" as a property somewhere (lowercase), fix it to "OrderStatuses"

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Convert all unbounded strings to longtext (MySQL-safe)
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string) && !property.IsKey() && property.GetMaxLength() == null)
                    {
                        property.SetColumnType("longtext");
                    }
                }
            }

            // Identity table MySQL type configuration
            builder.Entity<IdentityUser>(entity =>
            {
                entity.Property(u => u.Id).HasColumnType("varchar(255)");
                entity.Property(u => u.ConcurrencyStamp).HasColumnType("longtext");
                entity.Property(u => u.SecurityStamp).HasColumnType("longtext");
                entity.Property(u => u.PasswordHash).HasColumnType("longtext");
            });

            builder.Entity<IdentityRole>(entity =>
            {
                entity.Property(r => r.Id).HasColumnType("varchar(255)");
                entity.Property(r => r.ConcurrencyStamp).HasColumnType("longtext");
            });

            builder.Entity<IdentityRoleClaim<string>>(entity =>
            {
                entity.Property(rc => rc.RoleId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserClaim<string>>(entity =>
            {
                entity.Property(uc => uc.UserId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(ul => ul.UserId).HasColumnType("varchar(255)");
                entity.Property(ul => ul.LoginProvider).HasColumnType("varchar(255)");
                entity.Property(ul => ul.ProviderKey).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(ur => ur.UserId).HasColumnType("varchar(255)");
                entity.Property(ur => ur.RoleId).HasColumnType("varchar(255)");
            });

            builder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(ut => ut.UserId).HasColumnType("varchar(255)");
                entity.Property(ut => ut.LoginProvider).HasColumnType("varchar(255)");
                entity.Property(ut => ut.Name).HasColumnType("varchar(255)");
            });

            // Ensure MySQL compatibility with lowercase table names
            foreach (var entity in builder.Model.GetEntityTypes())
            {
                var tableName = entity.GetTableName();
                if (!string.IsNullOrEmpty(tableName))
                {
                    entity.SetTableName(tableName.ToLower());
                }
            }

            // Book ↔ Stock one-to-one
            builder.Entity<Stock>()
                .HasOne(s => s.Book)
                .WithOne(b => b.Stock)
                .HasForeignKey<Stock>(s => s.BookId);
        }
    }
}
