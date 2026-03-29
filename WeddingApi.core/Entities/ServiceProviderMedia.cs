namespace WeddingApi.core.Entities
{
    public class ServiceProviderMedia
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty; // Cloudinary ID للحذف
        public string MediaType { get; set; } = "image"; // image or video
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ServiceProvider ServiceProvider { get; set; } = null!;
    }
}
