namespace WeddingApi.core.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int BookingId { get; set; }
        public Booking Booking { get; set; } = null!;
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";
        public string Notes { get; set; }
        public DateTime PaidAt { get; set; } = DateTime.UtcNow;
    }
}
