namespace WeddingApi.core.DTOs.Payments
{
    public class PaymentDto
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
    }
}
