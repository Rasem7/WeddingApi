using Microsoft.AspNetCore.Identity;

namespace WeddingApi.core.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        public int ServiceProviderId { get; set; } // ربط بجدول ServiceProvider
        public string FullName { get; set; } = string.Empty;
        public string UserType { get; set; } = "client"; // admin, client, provider
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public ServiceProvider ServiceProvider { get; set; } // Navigation property to ServiceProvider
        public Client Client { get; set; } // Navigation property to Client
    }
}
