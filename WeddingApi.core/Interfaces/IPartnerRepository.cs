using WeddingApi.core.Common;
using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    // ربط العريس بالعروسة عن طريق كود دعوة. العمليات اللي بتعدّل الطرفين بتتم في Save واحد (ذرّي).
    public interface IPartnerRepository
    {
        Task<PartnerInvite?> GetActiveInviteAsync(int inviterClientId);
        // بيلغي أي كود شغّال قبل كده للعميل ده، وبيعمل كود جديد.
        Task<PartnerInvite> CreateInviteAsync(int inviterClientId, TimeSpan lifetime);
        Task<PartnerLinkResult> AcceptAsync(string code, int acceptingClientId);
        Task<bool> UnlinkAsync(int clientId);
    }
}
