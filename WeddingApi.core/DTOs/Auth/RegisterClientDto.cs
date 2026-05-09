using System.ComponentModel.DataAnnotations;

namespace WeddingApi.Core.DTOs.Auth
{
    public class RegisterClientDto
    {
        [Required(ErrorMessage = "Full name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "FullName must be between 3 and 100 characters.")]
        [RegularExpression(@"^[\p{L}\s\-']+$",
            ErrorMessage = "FullName can only contain letters, spaces, hyphens, and apostrophes.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        [StringLength(256, ErrorMessage = "Email cannot exceed 256 characters.")]
        public string Email { get; set; } = string.Empty;

        // ── NEW ───────────────────────────────────────────────────────
        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_\-.]+$",
            ErrorMessage = "UserName can only contain letters, digits, underscores, hyphens, and dots.")]
        public string UserName { get; set; } = string.Empty;
        // ─────────────────────────────────────────────────────────────

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required.")]
        [RegularExpression(@"^(groom|bride)$",
            ErrorMessage = "Gender must be either 'groom' or 'bride'.")]
        public string Gender { get; set; } = string.Empty;

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

        // ── NEW ───────────────────────────────────────────────────────
        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+?[0-9]{7,15}$",
            ErrorMessage = "Phone must be 7 to 15 digits and may start with +.")]
        public string Phone { get; set; } = string.Empty;
        // ─────────────────────────────────────────────────────────────

        [Required(ErrorMessage = "Budget is required.")]
        [Range(0.01, 99999999.99, ErrorMessage = "Budget must be between 0.01 and 99,999,999.99.")]
        public decimal Budget { get; set; }

        [Required(ErrorMessage = "Budget category is required.")]
        [RegularExpression(@"^(Economy|Standard|Premium|Luxury)$",
            ErrorMessage = "BudgetCategory must be one of: Economy, Standard, Premium, Luxury.")]
        public string BudgetCategory { get; set; } = "Standard";

        // ── NEW ───────────────────────────────────────────────────────
        [Required(ErrorMessage = "Address is required.")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "Address must be between 3 and 250 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-#/()]+$",
            ErrorMessage = "Address contains invalid characters.")]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "National ID is required.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "NationalId must be between 5 and 20 characters.")]
        [RegularExpression(@"^[A-Za-z0-9\-]+$",
            ErrorMessage = "NationalId can only contain letters, digits, and hyphens.")]
        public string NationalId { get; set; } = string.Empty;
        // ─────────────────────────────────────────────────────────────
    }
}