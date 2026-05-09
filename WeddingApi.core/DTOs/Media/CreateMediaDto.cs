using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.Media
{
    public class CreateMediaDto
    {
        [Required(ErrorMessage = "ServiceProviderId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be a positive number.")]
        public int ServiceProviderId { get; set; }

        [Required(ErrorMessage = "URL is required.")]
        [StringLength(2048, MinimumLength = 10, ErrorMessage = "Url must be between 10 and 2048 characters.")]
        [DataType(DataType.Url)]
        [RegularExpression(@"^https://res\.cloudinary\.com/[\w\-]+/(image|video)/upload/[\w\-./]+$",
            ErrorMessage = "Url must be a valid Cloudinary URL.")]
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
    }
}
