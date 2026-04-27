using Microsoft.AspNetCore.Identity;

namespace WeddingApi.core.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;
        public string UserType { get; set; } = "client"; // admin, client, provider
        public string Gender { get; set; } = string.Empty; // groom, bride (للعملاء بس)
        public string? NationalId { get; set; } // اختياري للعملاء
        public int? LinkedPartnerId { get; set; } // ربط العريس بالعروسة
        public string? BusinessName { get; set; } // لمزودي الخدمة
        public string? Category { get; set; } // لمزودي الخدمة
        public string ProviderStatus { get; set; } = "approved"; // pending, approved, rejected
        public int? ServiceProviderId { get; set; } // ربط بجدول ServiceProvider
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
