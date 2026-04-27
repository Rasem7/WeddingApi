using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WeddingApi.core.Entities;
using WeddingApi.infrastructure.Data;
using ServiceProvider = WeddingApi.core.Entities.ServiceProvider;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider services)
    {
        var context = services.GetRequiredService<WeddingDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<int>>>();

        // Apply migrations
        await context.Database.MigrateAsync();

        // 🔹 Roles
        if (!await roleManager.RoleExistsAsync("Admin"))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Admin" });

        if (!await roleManager.RoleExistsAsync("Client"))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Client" });

        if (!await roleManager.RoleExistsAsync("Provider"))
            await roleManager.CreateAsync(new IdentityRole<int> { Name = "Provider" });

        // 🔹 Admin User
        var adminEmail = "admin@wedding.com";

        var adminUser = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = "admin",
                Email = adminEmail,
                FullName = "Admin",
                UserType = "admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await userManager.CreateAsync(adminUser, "Admin@123");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }

        // 🔹 Service Providers (Static data عادي)
        if (!context.ServiceProviders.Any())
        {
            context.ServiceProviders.AddRange(
                new ServiceProvider { Id = 1, Name = "بالاس ويدينج هول", Category = "قاعات الأفراح", Location = "مدينة نصر، القاهرة", Rating = 4.9, ReviewCount = 234, PriceFrom = 30000, Phone = "01001234567", Description = "أفخم قاعات الأفراح في القاهرة", IsActive = true },
    new ServiceProvider { Id = 2, Name = "ستوديو ليلى فوتو", Category = "التصوير والفيديو", Location = "المعادي، القاهرة", Rating = 4.8, ReviewCount = 189, PriceFrom = 8000, Phone = "01112345678", Description = "تصوير احترافي لحفلات الزفاف", IsActive = true },
    new ServiceProvider { Id = 3, Name = "فلاورز باي نور", Category = "تنسيق الزهور", Location = "الزمالك، القاهرة", Rating = 4.7, ReviewCount = 156, PriceFrom = 5000, Phone = "01223456789", Description = "تنسيق زهور وديكور للأفراح", IsActive = true },
    new ServiceProvider { Id = 4, Name = "لافوشيه كيترينج", Category = "الكيترينج", Location = "التجمع الخامس", Rating = 4.6, ReviewCount = 312, PriceFrom = 120, Phone = "01334567890", Description = "أشهى المأكولات لحفل زفافك", IsActive = true },
    new ServiceProvider { Id = 5, Name = "جلوري برايدال", Category = "فساتين الزفاف", Location = "المهندسين، القاهرة", Rating = 4.9, ReviewCount = 421, PriceFrom = 5000, Phone = "01445678901", Description = "أجمل فساتين الزفاف", IsActive = true },
    new ServiceProvider { Id = 6, Name = "موسيقار بند", Category = "الموسيقى", Location = "وسط البلد، القاهرة", Rating = 4.7, ReviewCount = 98, PriceFrom = 8000, Phone = "01556789012", Description = "فرقة موسيقية متخصصة في حفلات الزفاف", IsActive = true },
    new ServiceProvider { Id = 7, Name = "جلام بيوتي", Category = "كوافير ومكياج", Location = "المعادي، القاهرة", Rating = 4.8, ReviewCount = 203, PriceFrom = 3000, Phone = "01667890123", Description = "متخصصون في إطلالات العرايس", IsActive = true }
);

            await context.SaveChangesAsync();
        }
    }
}