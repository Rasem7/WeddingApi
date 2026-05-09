using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.ServiceProviders
{
    public class UpdateServiceProviderDto
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s\-'&.]+$",
            ErrorMessage = "Name can only contain letters, digits, spaces, hyphens, ampersands, and dots.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(150, MinimumLength = 2, ErrorMessage = "BusinessName must be between 2 and 150 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s\-'&.()]+$",
            ErrorMessage = "BusinessName can only contain letters, digits, spaces, hyphens, ampersands, dots, and parentheses.")]
        public string BusinessName { get; set; } = string.Empty;

        [RegularExpression(@"^(Photography|Catering|Decoration|Music|Venue|Transportation|Beauty|Clothing|Cake|Invitation|Other)$",
            ErrorMessage = "Category must be one of: Photography, Catering, Decoration, Music, Venue, Transportation, Beauty, Clothing, Cake, Invitation, Other.")]
        public string Category { get; set; } = string.Empty;

        [StringLength(250, MinimumLength = 3, ErrorMessage = "Location must be between 3 and 250 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-#/()]+$",
            ErrorMessage = "Location contains invalid characters.")]
        public string Location { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+?[0-9]{7,15}$",
            ErrorMessage = "Phone must be 7 to 15 digits and may start with +.")]
        public string Phone { get; set; } = string.Empty;

        [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r&@]+$",
            ErrorMessage = "Description contains invalid characters.")]
        public string Description { get; set; } = string.Empty;

        [Range(0.01, 99999999.99, ErrorMessage = "PriceFrom must be between 0.01 and 99,999,999.99.")]
        [DataType(DataType.Currency)]
        public decimal? PriceFrom { get; set; }

        [RegularExpression(@"^(Active|Inactive|Suspended|Banned)$",
            ErrorMessage = "Status must be one of: Active, Inactive, Suspended, Banned.")]
        public string Status { get; set; } = string.Empty;
    }
}
