namespace WeddingApi.core.DTOs.TaskItems
{
    public class TaskItemDto
    {
        public int Id { get; set; }
        public int OwnerClientId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal? EstimatedCost { get; set; }
        public DateTime? DueDate { get; set; }
        public string AssignedTo { get; set; } = string.Empty;
        public bool IsOverdue { get; set; }
        public DateTime? CompletedAt { get; set; }
        public int? CompletedByClientId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
