using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.DTOs.TaskItems
{
    // كل الحقول اختيارية: اللي مش مبعوت (null) مبيتغيّرش.
    public class UpdateTaskItemDto
    {
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Title must be between 2 and 150 characters.")]
        public string Title { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters.")]
        public string Notes { get; set; }

        [RegularExpression(@"^(Low|Medium|High)$", ErrorMessage = "Priority must be one of: Low, Medium, High.")]
        public string Priority { get; set; }

        [RegularExpression(@"^(Todo|InProgress|Done)$", ErrorMessage = "Status must be one of: Todo, InProgress, Done.")]
        public string Status { get; set; }

        [Range(0, 99999999.99, ErrorMessage = "EstimatedCost must be between 0 and 99,999,999.99.")]
        public decimal? EstimatedCost { get; set; }

        public DateTime? DueDate { get; set; }

        [RegularExpression(@"^(Groom|Bride|Both)$", ErrorMessage = "AssignedTo must be one of: Groom, Bride, Both.")]
        public string AssignedTo { get; set; }

        // لمسح القيمة (لأن null معناها "متغيّرش")
        public bool? ClearEstimatedCost { get; set; }
        public bool? ClearDueDate { get; set; }
    }
}
