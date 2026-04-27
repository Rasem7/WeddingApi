namespace WeddingApi.core.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string GroomName { get; set; } = string.Empty;
        public string BrideName { get; set; } = string.Empty;
        public string GroomPhone { get; set; } = string.Empty;
        public string BridePhone { get; set; }
        public string Gender { get; set; } = string.Empty; // groom, bride (للعملاء بس)
        public string NationalId { get; set; } // اختياري للعملاء
        public int LinkedPartnerId { get; set; } // ربط العريس بالعروسة
        public decimal Budget { get; set; }
        public string BudgetCategory { get; set; } = "Standard";
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ApplicationUser User { get; set; } // Navigation property to ApplicationUser
        public DateTime CreatedAt { get; set; }
    }
}
