using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    public class ServiceProviderMedia
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ServiceProviderId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be a positive number.")]
        public int ServiceProviderId { get; set; }

        [Required(ErrorMessage = "URL is required.")]
        [StringLength(2048, MinimumLength = 10, ErrorMessage = "URL must be between 10 and 2048 characters.")]
        [DataType(DataType.Url)]
        [RegularExpression(@"^https://res\.cloudinary\.com/[\w\-]+/(image|video)/upload/[\w\-./]+$",
            ErrorMessage = "Url must be a valid Cloudinary URL (e.g., https://res.cloudinary.com/{cloud}/image/upload/{path}).")]
        public string Url { get; set; } = string.Empty;

        [Required(ErrorMessage = "PublicId is required.")]
        [StringLength(255, MinimumLength = 1, ErrorMessage = "PublicId must be between 1 and 255 characters.")]
        [RegularExpression(@"^[\w\-./]+$",
            ErrorMessage = "PublicId can only contain letters, digits, underscores, hyphens, dots, and forward slashes.")]
        public string PublicId { get; set; } = string.Empty;

        [Required(ErrorMessage = "MediaType is required.")]
        [RegularExpression(@"^(image|video)$",
            ErrorMessage = "MediaType must be either 'image' or 'video'.")]
        public string MediaType { get; set; } = "image";

        [Required(ErrorMessage = "CreatedAt is required.")]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(ServiceProviderMedia), nameof(ValidateCreatedAt))]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ServiceProvider ServiceProvider { get; set; } = null!;

        // ── Custom Validator ──────────────────────────────────────────
        public static ValidationResult ValidateCreatedAt(DateTime createdAt, ValidationContext context)
        {
            return createdAt > DateTime.UtcNow.AddMinutes(5)
                ? new ValidationResult("CreatedAt cannot be a future date.")
                : ValidationResult.Success;
        }
    }
}