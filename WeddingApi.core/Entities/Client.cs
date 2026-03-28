namespace WeddingApi.core.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string GroomName { get; set; } = string.Empty;
        public string BrideName { get; set; } = string.Empty;
        public string GroomPhone { get; set; } = string.Empty;
        public string? BridePhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal Budget { get; set; }
        public string BudgetCategory { get; set; } = "Standard";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
