using WeddingApi.core.DTOs.Availability;

namespace WeddingApi.core.Interfaces
{
    public interface IAvailabilityService
    {
        // بيتحقق إن اليوم ده متاح عند المزود: مش في الماضي، مش مقفول، ومعدّاش السعة اليومية.
        Task<AvailabilityResult> CheckAsync(int serviceProviderId, string category, DateTime date);
    }
}
