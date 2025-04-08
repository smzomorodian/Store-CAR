using Microsoft.EntityFrameworkCore;
using Domain.Model;
using Carproject.Model; // برای Information

namespace Infrustructure.Context
{
    public class CARdbcontext : DbContext
    {
        public CARdbcontext() { }
        public CARdbcontext(DbContextOptions<CARdbcontext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=CARDB;Trusted_Connection=True;");
            }
        }

        // public DbSet<User> informations { get; set; } // در ساختار TPT نمخواد که کلاس پدر رو درست کنی
        public DbSet<Moder> moders { get; set; }
        public DbSet<Seller> sellers { get; set; }
        public DbSet<Buyer> buyers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarCategory> CarCategories { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }

        public DbSet<Sale> Sales { get; set; }                // DbSet برای فروش‌ها

        public DbSet<Expense> Expenses { get; set; }          // DbSet برای هزینه‌ها
        public DbSet<OperatingExpense> OperatingExpenses { get; set; } // DbSet برای هزینه‌های عملیاتی
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // پیکربندی TPT
            modelBuilder.Entity<User>()
                .ToTable("Users");

            modelBuilder.Entity<Moder>()
                .ToTable("Moders") // جدول فرعی
                .HasBaseType<User>(); // وراثت TPT

            modelBuilder.Entity<Seller>()
                .ToTable("sellers")
                .HasBaseType<User>();

            modelBuilder.Entity<Buyer>()
                .ToTable("buyers")
                .HasBaseType<User>();

            base.OnModelCreating(modelBuilder);
        }
    }
}