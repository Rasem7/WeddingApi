using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.Bookings
{
    public class CreateBookingDto
    {
        [Required(ErrorMessage = "ClientId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ClientId must be a positive number.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Wedding date is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(CreateBookingDto), nameof(ValidateFutureDate))]
        public DateTime WeddingDate { get; set; }

        [Required(ErrorMessage = "Wedding time is required.")]
        [RegularExpression(@"^(0[1-9]|1[0-2]):[0-5][0-9]\s(AM|PM)$",
            ErrorMessage = "WeddingTime must be in format hh:mm AM/PM (e.g., 07:30 PM).")]
        public string WeddingTime { get; set; } = string.Empty;

        [Required(ErrorMessage = "Venue is required.")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Venue must be between 3 and 200 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-#/()]+$",
            ErrorMessage = "Venue contains invalid characters.")]
        public string Venue { get; set; } = string.Empty;

        [Required(ErrorMessage = "Guest count is required.")]
        [Range(1, 10000, ErrorMessage = "GuestCount must be between 1 and 10,000.")]
        public int GuestCount { get; set; }

        [Required(ErrorMessage = "Event type is required.")]
        [RegularExpression(@"^(FullWedding|Reception|Engagement|Birthday|Corporate|Other)$",
            ErrorMessage = "EventType must be one of: FullWedding, Reception, Engagement, Birthday, Corporate, Other.")]
        public string EventType { get; set; } = "FullWedding";

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r]*$",
            ErrorMessage = "Notes contain invalid characters.")]
        public string Notes { get; set; } = string.Empty;

        // الباقة المختارة. لو اتبعتت، السعر بيتحسب تلقائيًا من الباقة ويتجاهل TotalAmount.
        [Range(1, int.MaxValue, ErrorMessage = "ServiceId must be a positive number.")]
        public int? ServiceId { get; set; }

        // مطلوب فقط لو مفيش ServiceId (حجز يدوي بسعر متفق عليه).
        public decimal TotalAmount { get; set; }

        public static ValidationResult ValidateFutureDate(DateTime date, ValidationContext context)
        {
            return date.Date < DateTime.UtcNow.Date
                ? new ValidationResult("WeddingDate must be today or a future date.")
                : ValidationResult.Success;
        }
    }
}
