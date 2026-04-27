namespace WeddingApi.core.DTOs.Auth
{
    public class RegisterClientDto
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string Gender { get; set; }
        public string NationalId { get; set; }
    }
}
