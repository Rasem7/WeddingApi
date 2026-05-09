using System.ComponentModel.DataAnnotations;

public class UpdateProfileDto
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "GroomName must be between 3 and 100 characters.")]
    [RegularExpression(@"^[\p{L}\s\-']+$",
        ErrorMessage = "GroomName can only contain letters, spaces, hyphens, and apostrophes.")]
    public string GroomName { get; set; } = string.Empty;

    [StringLength(100, MinimumLength = 3, ErrorMessage = "BrideName must be between 3 and 100 characters.")]
    [RegularExpression(@"^[\p{L}\s\-']+$",
        ErrorMessage = "BrideName can only contain letters, spaces, hyphens, and apostrophes.")]
    public string BrideName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "GroomPhone must be 7 to 15 digits and may start with +.")]
    public string GroomPhone { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Invalid phone number.")]
    [RegularExpression(@"^\+?[0-9]{7,15}$",
        ErrorMessage = "BridePhone must be 7 to 15 digits and may start with +.")]
    public string BridePhone { get; set; } = string.Empty;

    [Range(0.01, 99999999.99, ErrorMessage = "Budget must be between 0.01 and 99,999,999.99.")]
    public decimal? Budget { get; set; }

    [RegularExpression(@"^(Economy|Standard|Premium|Luxury)$",
        ErrorMessage = "BudgetCategory must be one of: Economy, Standard, Premium, Luxury.")]
    public string BudgetCategory { get; set; } = string.Empty;

    [StringLength(250, ErrorMessage = "Address cannot exceed 250 characters.")]
    [RegularExpression(@"^[\p{L}0-9\s,.\-#/()]+$",
        ErrorMessage = "Address contains invalid characters.")]
    public string Address { get; set; } = string.Empty;
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string NationalId { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
}