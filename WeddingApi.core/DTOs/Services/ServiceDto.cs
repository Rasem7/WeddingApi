namespace WeddingApi.core.DTOs.Services
{
    public class ServiceDto
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
