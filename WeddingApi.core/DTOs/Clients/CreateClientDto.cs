namespace WeddingApi.core.DTOs.Clients
{
    public class CreateClientDto
    {
        public string GroomName { get; set; } = string.Empty;
        public string BrideName { get; set; } = string.Empty;
        public string GroomPhone { get; set; } = string.Empty;
        public string? BridePhone { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public decimal Budget { get; set; }
    }
}
