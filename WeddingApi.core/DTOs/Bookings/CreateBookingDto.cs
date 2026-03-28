namespace WeddingApi.core.DTOs.Bookings
{
    public class CreateBookingDto
    {
        public int ClientId { get; set; }
        public DateTime WeddingDate { get; set; }
        public string? WeddingTime { get; set; }
        public string? Venue { get; set; }
        public int GuestCount { get; set; }
        public string EventType { get; set; } = "FullWedding";
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
    }
}
