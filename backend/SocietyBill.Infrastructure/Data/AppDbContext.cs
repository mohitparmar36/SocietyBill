using Microsoft.EntityFrameworkCore;
using SocietyBill.Domain.Entities;
using SocietyBill.Application.Interfaces.Identity;

namespace SocietyBill.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        private readonly ITenantProvider _tenantProvider;

        public AppDbContext(DbContextOptions<AppDbContext> options, ITenantProvider tenantProvider)
            : base(options)
        {
            _tenantProvider = tenantProvider;
        }

        public DbSet<Society> Societies { get; set; } = null!;
        public DbSet<Flat> Flats { get; set; } = null!;
        public DbSet<Bill> Bills { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Set default PostgreSQL schema
            modelBuilder.HasDefaultSchema("society_bill");

            // Global Query Filters for Multi-Tenancy
            modelBuilder.Entity<Flat>()
                .HasQueryFilter(x => x.SocietyId == _tenantProvider.SocietyId);
            modelBuilder.Entity<Bill>()
                .HasQueryFilter(x => x.Flat.SocietyId == _tenantProvider.SocietyId);

            // Property Configurations
            modelBuilder.Entity<Society>(entity =>
            {
                entity.ToTable("Societies");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<Flat>(entity =>
            {
                entity.ToTable("Flats");
                entity.Property(e => e.FlatNumber).IsRequired().HasMaxLength(50);
                entity.Property(e => e.OwnerName).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Auth0UserId).HasMaxLength(100);

                // Indexes
                entity.HasIndex(x => x.SocietyId);
                entity.HasIndex(x => x.Auth0UserId);
            });

            modelBuilder.Entity<Bill>(entity =>
            {
                entity.ToTable("Bills");
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");
                
                // Indexes
                entity.HasIndex(x => x.FlatId);
                entity.HasIndex(x => x.Year);
            });
        }
    }
}