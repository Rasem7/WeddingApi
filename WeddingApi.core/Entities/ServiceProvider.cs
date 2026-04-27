namespace WeddingApi.core.Entities
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public string Location { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public decimal PriceFrom { get; set; }
        public string Description { get; set; }
        public string Phone { get; set; }
        public string Status { get; set; }
        public string BusinessName { get; set; } // لمزودي الخدمة
        public bool IsActive { get; set; } = true;
        public string ProviderStatus { get; set; } = "approved"; // pending, approved, rejected
        public ApplicationUser User { get; set; } // Navigation property to ApplicationUser
    }
}
