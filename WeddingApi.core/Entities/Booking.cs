using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    public class Booking
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ClientId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ClientId must be a positive number.")]
        public int ClientId { get; set; }

        public Client Client { get; set; } = null!;

        [Required(ErrorMessage = "Wedding date is required.")]
        [DataType(DataType.Date)]
        [CustomValidation(typeof(Booking), nameof(ValidateFutureDate))]
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

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression(@"^(Pending|Confirmed|Cancelled|Completed|Refunded)$",
            ErrorMessage = "Status must be one of: Pending, Confirmed, Cancelled, Completed, Refunded.")]
        public string Status { get; set; } = "Pending";

        [Required(ErrorMessage = "Total amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "TotalAmount must be between 0.01 and 9,999,999.99.")]
        [DataType(DataType.Currency)]
        public decimal TotalAmount { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r]*$",
            ErrorMessage = "Notes contain invalid characters.")]
        public string Notes { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Payment> Payments { get; set; } = new List<Payment>();

        // ── Custom Validator ──────────────────────────────────────────
        public static ValidationResult ValidateFutureDate(DateTime date, ValidationContext context)
        {
            return date.Date < DateTime.UtcNow.Date
                ? new ValidationResult("WeddingDate must be today or a future date.")
                : ValidationResult.Success;
        }
    }
}