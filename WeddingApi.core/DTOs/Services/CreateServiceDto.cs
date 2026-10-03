using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.Services
{
    public class CreateServiceDto
    {
        [Required(ErrorMessage = "ServiceProviderId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be a positive number.")]
        public int ServiceProviderId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s\-'&.()]+$",
            ErrorMessage = "Name can only contain letters, digits, spaces, hyphens, ampersands, dots, and parentheses.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 99999999.99, ErrorMessage = "Price must be between 0.01 and 99,999,999.99.")]
        public decimal Price { get; set; }
    }
}
