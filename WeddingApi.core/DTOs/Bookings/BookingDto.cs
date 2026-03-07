namespace WeddingApi.core.DTOs.Bookings
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string GroomName { get; set; } = string.Empty;
        public string BrideName { get; set; } = string.Empty;
        public DateTime WeddingDate { get; set; }
        public string? Venue { get; set; }
        public int GuestCount { get; set; }
        public string EventType { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
    }
}
