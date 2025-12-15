using Microsoft.EntityFrameworkCore;
using RentSystem.Domain.Entities;

namespace RentSystem.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Property> Properties => Set<Property>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
        public DbSet<PasswordResetCode> PasswordResetCodes => Set<PasswordResetCode>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configurations
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .IsRequired();

                entity.HasIndex(e => e.Email)
                    .IsUnique();

                entity.Property(e => e.Roles)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                    );

                entity.HasMany(u => u.Properties)
                    .WithOne(p => p.Owner)
                    .HasForeignKey(p => p.OwnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(u => u.Bookings)
                    .WithOne(b => b.Renter)
                    .HasForeignKey(b => b.RenterId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.Reviews)
                    .WithOne(r => r.User)
                    .HasForeignKey(r => r.UserId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasMany(u => u.RefreshTokens)
                    .WithOne(rt => rt.User)
                    .HasForeignKey(rt => rt.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Property configurations
            modelBuilder.Entity<Property>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Title)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(100);

                entity.Property(e => e.Location)
                    .HasMaxLength(100);

                entity.Property(e => e.PricePerNight)
                    .HasColumnType("decimal(18,2)");

                entity.HasMany(p => p.Bookings)
                    .WithOne(b => b.Property)
                    .HasForeignKey(b => b.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(p => p.Reviews)
                    .WithOne(r => r.Property)
                    .HasForeignKey(r => r.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Booking configurations
            modelBuilder.Entity<Booking>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.StartDate)
                    .IsRequired();

                entity.Property(e => e.EndDate)
                    .IsRequired();
            });

            // Review configurations
            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.Property(e => e.CreatedAt)
                    .IsRequired();
            });

            // RefreshToken configurations
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Token)
                    .IsRequired();

                entity.HasIndex(e => e.Token)
                    .IsUnique();
            });

            // PasswordResetCode configurations
            modelBuilder.Entity<PasswordResetCode>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .IsRequired();
            });
        }
    }
}
