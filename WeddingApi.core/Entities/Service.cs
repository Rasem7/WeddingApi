using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    // خدمة/باقة فعلية يقدّمها مزود الخدمة، بسعرها الخاص.
    // مزود الخدمة الواحد ممكن يكون عنده أكتر من Service (مثلاً قاعة عندها 3 باقات مختلفة).
    // ServiceProvider.PriceFrom فضل زي ما هو كـ"سعر يبدأ من" للعرض السريع في نتائج البحث،
    // أما التفاصيل والأسعار الحقيقية لكل باقة فبتتخزن هنا.
    public class Service
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ServiceProviderId is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "ServiceProviderId must be a positive number.")]
        public int ServiceProviderId { get; set; }

        public ServiceProvider ServiceProvider { get; set; } = null!;

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 150 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s\-'&.()]+$",
            ErrorMessage = "Name can only contain letters, digits, spaces, hyphens, ampersands, dots, and parentheses.")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        [RegularExpression(@"^[\p{L}0-9\s,.\-!?()'""#/\n\r&@]*$",
            ErrorMessage = "Description contains invalid characters.")]
        public string Description { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, 99999999.99, ErrorMessage = "Price must be between 0.01 and 99,999,999.99.")]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
