using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;

namespace WeddingApi.infrastructure.Data;

public class WeddingDbContext : DbContext
{
    public WeddingDbContext(DbContextOptions<WeddingDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Client → Bookings
        modelBuilder.Entity<Booking>()
            .HasOne(b => b.Client)
            .WithMany(c => c.Bookings)
            .HasForeignKey(b => b.ClientId)
            .OnDelete(DeleteBehavior.Cascade);

        // Booking → Payments
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Booking)
            .WithMany(b => b.Payments)
            .HasForeignKey(p => p.BookingId)
            .OnDelete(DeleteBehavior.Cascade);

        // Decimal precision
        modelBuilder.Entity<Client>()
            .Property(c => c.Budget).HasPrecision(18, 2);
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalAmount).HasPrecision(18, 2);
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount).HasPrecision(18, 2);
        modelBuilder.Entity<ServiceProvider>()
            .Property(v => v.PriceFrom).HasPrecision(18, 2);

        // Seed Admin User
        modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "$2a$11$pTMvOtyhcOiDXHiCqzIkdeN2EugPLJka.OaO0dB2N95VdHv1kAFMe",
            Role = "Admin",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
    }
}