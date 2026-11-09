using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    // مهمة في قائمة تجهيزات الزفاف. القائمة مشتركة بين العريس والعروسة:
    // كل مهمة ليها صاحب (OwnerClientId = اللي أنشأها)، والطرف المربوط بيه (LinkedPartnerId)
    // بيشوفها ويعدّل عليها كمان. مفيش جدول "قائمة" منفصل — قائمة الزوجين هي كل مهام الاتنين.
    public class TaskItem
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "OwnerClientId must be a positive number.")]
        public int OwnerClientId { get; set; }

        public Client OwnerClient { get; set; } = null!;

        [Required(ErrorMessage = "Title is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 150 characters.")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string Notes { get; set; } = string.Empty;

        [RegularExpression(@"^(Low|Medium|High)$", ErrorMessage = "Priority must be one of: Low, Medium, High.")]
        public string Priority { get; set; } = "Medium";

        [RegularExpression(@"^(Todo|InProgress|Done)$", ErrorMessage = "Status must be one of: Todo, InProgress, Done.")]
        public string Status { get; set; } = "Todo";

        [Range(0, 99999999.99, ErrorMessage = "EstimatedCost must be between 0 and 99,999,999.99.")]
        [DataType(DataType.Currency)]
        public decimal? EstimatedCost { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }

        // مين مسؤول عن المهمة
        [RegularExpression(@"^(Groom|Bride|Both)$", ErrorMessage = "AssignedTo must be one of: Groom, Bride, Both.")]
        public string AssignedTo { get; set; } = "Both";

        public DateTime? CompletedAt { get; set; }

        // مين من الاتنين علّم المهمة "تمت" (Client.Id)
        public int? CompletedByClientId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
