namespace WeddingApi.core.Entities
{
    public class Booking
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public DateTime WeddingDate { get; set; }
        public string? WeddingTime { get; set; }
        public string? Venue { get; set; }
        public int GuestCount { get; set; }
        public string EventType { get; set; } = "FullWedding";
        public string Status { get; set; } = "Pending";
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
