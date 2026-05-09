using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    public class Payment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "BookingId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BookingId must be a positive number.")]
        public int BookingId { get; set; }

        public Booking Booking { get; set; } = null!;

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be between 0.01 and 9,999,999.99.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression(@"^(Cash|CreditCard|DebitCard|BankTransfer|Cheque|Online)$",
            ErrorMessage = "Method must be one of: Cash, CreditCard, DebitCard, BankTransfer, Cheque, Online.")]
        public string Method { get; set; } = "Cash";

        [Required(ErrorMessage = "Notes field cannot be null.")]
        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r]*$",
            ErrorMessage = "Notes contain invalid characters.")]
        public string Notes { get; set; } = string.Empty;

        [Required(ErrorMessage = "PaidAt date is required.")]
        [DataType(DataType.DateTime)]
        [CustomValidation(typeof(Payment), nameof(ValidatePaidAt))]
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;

        // ── Custom Validator ──────────────────────────────────────────
        public static ValidationResult ValidatePaidAt(DateTime paidAt, ValidationContext context)
        {
            return paidAt > DateTime.UtcNow.AddMinutes(5)
                ? new ValidationResult("PaidAt cannot be a future date.")
                : ValidationResult.Success;
        }
    }
}