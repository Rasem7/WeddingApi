using System.ComponentModel.DataAnnotations;

public class CreateClientDto
{
    [Required(ErrorMessage = "Groom name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "GroomName must be between 3 and 100 characters.")]
    [RegularExpression(@"^[\p{L}\s\-']+$",
        ErrorMessage = "GroomName can only contain letters, spaces, hyphens, and apostrophes.")]
    public string GroomName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bride name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "BrideName must be between 3 and 100 characters.")]
    [RegularExpression(@"^[\p{L}\s\-']+$",
        ErrorMessage = "BrideName can only contain letters, spaces, hyphens, and apostrophes.")]
    public string BrideName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Groom phone is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "GroomPhone must be 7 to 15 digits and may start with +.")]
    public string GroomPhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Bride phone is required.")]
    [Phone(ErrorMessage = "Invalid phone number.")]
    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "BridePhone must be 7 to 15 digits and may start with +.")]
    public string BridePhone { get; set; } = string.Empty;

    [Required(ErrorMessage = "Gender is required.")]
    [RegularExpression(@"^(groom|bride)$",
        ErrorMessage = "Gender must be either 'groom' or 'bride'.")]
    public string Gender { get; set; } = string.Empty;

    [StringLength(20, MinimumLength = 5, ErrorMessage = "NationalId must be between 5 and 20 characters.")]
    [RegularExpression(@"^[A-Za-z0-9\-]+$",
        ErrorMessage = "NationalId can only contain letters, digits, and hyphens.")]
    public string NationalId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Budget is required.")]
    [Range(0.01, 99999999.99, ErrorMessage = "Budget must be between 0.01 and 99,999,999.99.")]
    public decimal Budget { get; set; }

    [Required(ErrorMessage = "Budget category is required.")]
    [RegularExpression(@"^(Economy|Standard|Premium|Luxury)$",
        ErrorMessage = "BudgetCategory must be one of: Economy, Standard, Premium, Luxury.")]
    public string BudgetCategory { get; set; } = "Standard";
}