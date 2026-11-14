using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.Partners
{
    public class AcceptInviteDto
    {
        [Required(ErrorMessage = "Code is required.")]
        [StringLength(14, MinimumLength = 6, ErrorMessage = "Invalid code.")]
        public string Code { get; set; } = string.Empty;
    }
}
