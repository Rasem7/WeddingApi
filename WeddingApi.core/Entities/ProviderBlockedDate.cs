using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    // يوم مقفول من مزود الخدمة (إجازة، صيانة، ارتباط خارجي...). أي يوم مش مقفول
    // ومفيش عليه حجز بيوصل للسعة اليومية بيعتبر متاح.
    public class ProviderBlockedDate
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be a positive number.")]
        public int ServiceProviderId { get; set; }

        public ServiceProvider ServiceProvider { get; set; } = null!;

        // التاريخ بس (من غير وقت).
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [StringLength(250, ErrorMessage = "Reason cannot exceed 250 characters.")]
        public string Reason { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
