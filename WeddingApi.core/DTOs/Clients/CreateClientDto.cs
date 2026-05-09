namespace WeddingApi.core.DTOs.Clients
{
    public class CreateClientDto
    {
        public string GroomName { get; set; }
        public string BrideName { get; set; }
        public string GroomPhone { get; set; }
        public string BridePhone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal Budget { get; set; }
    }
}
