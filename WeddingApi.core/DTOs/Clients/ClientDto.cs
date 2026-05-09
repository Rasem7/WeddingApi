public class ClientDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string GroomName { get; set; } = string.Empty;
    public string BrideName { get; set; } = string.Empty;
    public string GroomPhone { get; set; } = string.Empty;
    public string BridePhone { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string NationalId { get; set; } = string.Empty;
    public decimal Budget { get; set; }
    public string BudgetCategory { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}