using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.Availability
{
    public class BlockDatesDto
    {
        [Required(ErrorMessage = "ServiceProviderId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be a positive number.")]
        public int ServiceProviderId { get; set; }

        [Required(ErrorMessage = "StartDate is required.")]
        public DateTime StartDate { get; set; }

        // اختياري: لو فاضي بيتقفل يوم واحد (StartDate) بس.
        public DateTime? EndDate { get; set; }

        [StringLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string Reason { get; set; } = string.Empty;
    }
}
