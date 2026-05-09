using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.Bookings
{
    public class UpdateBookingDto
    {
        [DataType(DataType.Date)]
        [CustomValidation(typeof(UpdateBookingDto), nameof(ValidateFutureDate))]
        public DateTime? WeddingDate { get; set; }

        [RegularExpression(@"^(0[1-9]|1[0-2]):[0-5][0-9]\s(AM|PM)$",
            ErrorMessage = "WeddingTime must be in format hh:mm AM/PM (e.g., 07:30 PM).")]
        public string WeddingTime { get; set; } = string.Empty;

        [StringLength(200, MinimumLength = 3, ErrorMessage = "Venue must be between 3 and 200 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-#/()]+$",
            ErrorMessage = "Venue contains invalid characters.")]
        public string Venue { get; set; } = string.Empty;

        [Range(1, 10000, ErrorMessage = "GuestCount must be between 1 and 10,000.")]
        public int? GuestCount { get; set; }

        [RegularExpression(@"^(FullWedding|Reception|Engagement|Birthday|Corporate|Other)$",
            ErrorMessage = "EventType must be one of: FullWedding, Reception, Engagement, Birthday, Corporate, Other.")]
        public string EventType { get; set; } = string.Empty;

        [RegularExpression(@"^(Pending|Confirmed|Cancelled|Completed|Refunded)$",
            ErrorMessage = "Status must be one of: Pending, Confirmed, Cancelled, Completed, Refunded.")]
        public string Status { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r]*$",
            ErrorMessage = "Notes contain invalid characters.")]
        public string Notes { get; set; } = string.Empty;

        public static ValidationResult ValidateFutureDate(DateTime? date, ValidationContext context)
        {
            if (date.HasValue && date.Value.Date < DateTime.UtcNow.Date)
                return new ValidationResult("WeddingDate must be today or a future date.");
            return ValidationResult.Success;
        }
    }
}
