using System.ComponentModel.DataAnnotations;

namespace WeddingApi.core.Entities
{
    // كود دعوة بيطلعه عميل (العريس أو العروسة) ويديه للطرف التاني عشان يتربطوا.
    // الكود مؤقت (بينتهي) ويتستخدم مرة واحدة.
    public class PartnerInvite
    {
        public int Id { get; set; }

        [Required]
        public int InviterClientId { get; set; }

        public Client InviterClient { get; set; } = null!;

        [Required]
        [StringLength(12)]
        public string Code { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        public DateTime? UsedAt { get; set; }

        public int? UsedByClientId { get; set; }
    }
}
