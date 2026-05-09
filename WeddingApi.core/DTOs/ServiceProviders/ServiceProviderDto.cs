namespace WeddingApi.core.DTOs.ServiceProviders
{
    public class ServiceProviderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string BusinessName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal PriceFrom { get; set; }
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string ProviderStatus { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
