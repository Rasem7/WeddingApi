namespace WeddingApi.core.DTOs.Auth;

public class UpdateProfileDto
{
    public string FullName { get; set; }
    public string Phone { get; set; }
    public string NationalId { get; set; }
    public string Location { get; set; }
    public string Description { get; set; }
}