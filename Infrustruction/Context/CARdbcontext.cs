using Microsoft.EntityFrameworkCore;
using Domain.Model; // برای Information

namespace Infrustruction.Context
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

        public DbSet<User> informations { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarCategory> CarCategories { get; set; }
    }
}