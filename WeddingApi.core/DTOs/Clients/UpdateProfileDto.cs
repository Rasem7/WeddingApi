namespace WeddingApi.core.DTOs.Auth;

public class UpdateProfileDto
{
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string? NationalId { get; set; }
    public string? Location { get; set; }
    public string? Description { get; set; }
}