using System.ComponentModel.DataAnnotations;
namespace WeddingApi.core.DTOs.Payments
{
    public class CreatePaymentDto
    {
        [Required(ErrorMessage = "BookingId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "BookingId must be a positive number.")]
        public int BookingId { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, 9999999.99, ErrorMessage = "Amount must be between 0.01 and 9,999,999.99.")]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment method is required.")]
        [RegularExpression(@"^(Cash|CreditCard|DebitCard|BankTransfer|Cheque|Online)$",
            ErrorMessage = "Method must be one of: Cash, CreditCard, DebitCard, BankTransfer, Cheque, Online.")]
        public string Method { get; set; } = "Cash";

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r]*$",
            ErrorMessage = "Notes contain invalid characters.")]
        public string Notes { get; set; } = string.Empty;
    }
}