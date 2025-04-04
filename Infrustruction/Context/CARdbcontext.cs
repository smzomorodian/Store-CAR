﻿using Microsoft.EntityFrameworkCore;
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

       // public DbSet<User> informations { get; set; } // در ساختار TPT نمخواد که کلاس پدر رو درست کنی
        public DbSet<Moder> moders { get; set; }
        public DbSet<Seller> sellers { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<CarCategory> CarCategories { get; set; }
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

            base.OnModelCreating(modelBuilder);
        }
    }
}