using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    public class ApplicationUser : IdentityUser<int>
    {
        [Range(0, int.MaxValue, ErrorMessage = "ServiceProviderId must be a non-negative number.")]
        public int ServiceProviderId { get; set; }

        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Full name must be between 3 and 100 characters.")]
        [RegularExpression(@"^[\p{L}\s\-']+$", ErrorMessage = "Full name can only contain letters, spaces, hyphens, and apostrophes.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "User type is required.")]
        [RegularExpression(@"^(admin|client|provider)$", ErrorMessage = "UserType must be one of: admin, client, provider.")]
        public string UserType { get; set; } = "client";

        [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-#/]+$", ErrorMessage = "Address contains invalid characters.")]
        public string Address { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public bool IsActive { get; set; } = true;

        public ServiceProvider ServiceProvider { get; set; }
        public Client Client { get; set; }
    }
}