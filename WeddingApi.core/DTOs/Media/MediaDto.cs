namespace WeddingApi.core.DTOs.Media
{
    public class MediaDto
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public string Url { get; set; } = string.Empty;
        public string PublicId { get; set; } = string.Empty;
        public string MediaType { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
