namespace WeddingApi.core.DTOs.Bookings
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string GroomName { get; set; }
        public string BrideName { get; set; }
        public DateTime WeddingDate { get; set; }
        public string Venue { get; set; }
        public int GuestCount { get; set; }
        public string EventType { get; set; }
        public string Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
