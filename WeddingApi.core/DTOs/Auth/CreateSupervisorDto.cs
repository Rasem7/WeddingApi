using System.ComponentModel.DataAnnotations;

namespace WeddingApi.Core.DTOs.Auth
{
    public class CreateSupervisorDto
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

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "UserName must be between 3 and 50 characters.")]
        [RegularExpression(@"^[A-Za-z0-9_\-.]+$",
            ErrorMessage = "UserName can only contain letters, digits, underscores, hyphens, and dots.")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
            ErrorMessage = "Password must contain at least one uppercase, one lowercase, one digit, and one special character.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirm password is required.")]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [RegularExpression(@"^\+?[0-9]{7,15}$",
            ErrorMessage = "Phone must be 7 to 15 digits and may start with +.")]
        public string Phone { get; set; } = string.Empty;
    }
}
