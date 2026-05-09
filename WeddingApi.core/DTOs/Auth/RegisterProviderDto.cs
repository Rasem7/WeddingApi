namespace WeddingApi.core.DTOs.Auth
{
    public class RegisterProviderDto
    {
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string BusinessName { get; set; }
        public string Category { get; set; }
    }
}
