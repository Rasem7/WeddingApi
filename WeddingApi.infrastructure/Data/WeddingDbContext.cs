using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;

namespace WeddingApi.infrastructure.Data;

public class WeddingDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public WeddingDbContext(DbContextOptions<WeddingDbContext> options): base(options) { }
    
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<ServiceProvider> ServiceProviders => Set<ServiceProvider>();
    public DbSet<ServiceProviderMedia> ServiceProviderMedias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        base.OnModelCreating(modelBuilder);
        // ApplicationUser config
        modelBuilder.Entity<ApplicationUser>()
            .HasIndex(u => u.NationalId);

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
        modelBuilder.Entity<ServiceProvider>()
           .Property(s => s.Rating)
           .HasPrecision(3, 1);
        modelBuilder.Entity<ServiceProviderMedia>()
    .HasOne(m => m.ServiceProvider)
    .WithMany()
    .HasForeignKey(m => m.ServiceProviderId)
    .OnDelete(DeleteBehavior.Cascade);
        // Seed Admin User
        /*modelBuilder.Entity<User>().HasData(new User
        {
            Id = 1,
            Username = "admin",
            PasswordHash = "$2a$11$pTMvOtyhcOiDXHiCqzIkdeN2EugPLJka.OaO0dB2N95VdHv1kAFMe",
            Role = "Admin",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });*/

        // Seed Roles
        /*modelBuilder.Entity<IdentityRole<int>>().HasData(
            new IdentityRole<int> { Id = 1, Name = "Admin", NormalizedName = "ADMIN" },
            new IdentityRole<int> { Id = 2, Name = "Client", NormalizedName = "CLIENT" },
            new IdentityRole<int> { Id = 3, Name = "Provider", NormalizedName = "PROVIDER" }
        );*/

        // Seed Admin User
        /*var hasher = new PasswordHasher<ApplicationUser>();
        var admin = new ApplicationUser
        {
            Id = 1,
            UserName = "admin",
            NormalizedUserName = "ADMIN",
            Email = "admin@wedding.com",
            NormalizedEmail = "ADMIN@WEDDING.COM",
            FullName = "Admin",
            UserType = "admin",
            IsActive = true,
            SecurityStamp = "STATIC_STAMP_001",
            CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        };*/
        /*admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");
        modelBuilder.Entity<ApplicationUser>().HasData(admin);*/

        // Seed Admin Role
        /*modelBuilder.Entity<IdentityUserRole<int>>().HasData(
            new IdentityUserRole<int> { UserId = 1, RoleId = 1 }
        );*/
        // Seed Admin User
        /*modelBuilder.Entity<ServiceProvider>().HasData(
    new ServiceProvider { Id = 1, Name = "بالاس ويدينج هول", Category = "قاعات الأفراح", Location = "مدينة نصر، القاهرة", Rating = 4.9, ReviewCount = 234, PriceFrom = 30000, Phone = "01001234567", Description = "أفخم قاعات الأفراح في القاهرة", IsActive = true },
    new ServiceProvider { Id = 2, Name = "ستوديو ليلى فوتو", Category = "التصوير والفيديو", Location = "المعادي، القاهرة", Rating = 4.8, ReviewCount = 189, PriceFrom = 8000, Phone = "01112345678", Description = "تصوير احترافي لحفلات الزفاف", IsActive = true },
    new ServiceProvider { Id = 3, Name = "فلاورز باي نور", Category = "تنسيق الزهور", Location = "الزمالك، القاهرة", Rating = 4.7, ReviewCount = 156, PriceFrom = 5000, Phone = "01223456789", Description = "تنسيق زهور وديكور للأفراح", IsActive = true },
    new ServiceProvider { Id = 4, Name = "لافوشيه كيترينج", Category = "الكيترينج", Location = "التجمع الخامس", Rating = 4.6, ReviewCount = 312, PriceFrom = 120, Phone = "01334567890", Description = "أشهى المأكولات لحفل زفافك", IsActive = true },
    new ServiceProvider { Id = 5, Name = "جلوري برايدال", Category = "فساتين الزفاف", Location = "المهندسين، القاهرة", Rating = 4.9, ReviewCount = 421, PriceFrom = 5000, Phone = "01445678901", Description = "أجمل فساتين الزفاف", IsActive = true },
    new ServiceProvider { Id = 6, Name = "موسيقار بند", Category = "الموسيقى", Location = "وسط البلد، القاهرة", Rating = 4.7, ReviewCount = 98, PriceFrom = 8000, Phone = "01556789012", Description = "فرقة موسيقية متخصصة في حفلات الزفاف", IsActive = true },
    new ServiceProvider { Id = 7, Name = "جلام بيوتي", Category = "كوافير ومكياج", Location = "المعادي، القاهرة", Rating = 4.8, ReviewCount = 203, PriceFrom = 3000, Phone = "01667890123", Description = "متخصصون في إطلالات العرايس", IsActive = true }
);
        */
    }


}