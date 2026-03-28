namespace WeddingApi.core.Entities
{
    public class ServiceProvider
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int ReviewCount { get; set; }
        public decimal PriceFrom { get; set; }
        public string? Description { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
