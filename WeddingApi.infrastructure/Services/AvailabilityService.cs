using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Availability;
using WeddingApi.core.Interfaces;

namespace WeddingApi.infrastructure.Services;

public class AvailabilityService : IAvailabilityService
{
    private readonly IUnitOfWorks _unitOfWork;
    public AvailabilityService(IUnitOfWorks unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<AvailabilityResult> CheckAsync(int serviceProviderId, string category, DateTime date)
    {
        var day = date.Date;

        if (day < DateTime.UtcNow.Date)
            return AvailabilityResult.No("التاريخ المطلوب في الماضي.");

        if (await _unitOfWork.Availability.IsBlockedAsync(serviceProviderId, day))
            return AvailabilityResult.No("مزود الخدمة غير متاح في هذا اليوم.");

        var capacity = AvailabilityPolicy.DailyCapacity(category);
        if (capacity.HasValue)
        {
            var booked = await _unitOfWork.Bookings.CountActiveForProviderOnDateAsync(serviceProviderId, day);
            if (booked >= capacity.Value)
                return AvailabilityResult.No("هذا اليوم محجوز بالفعل عند مزود الخدمة.");
        }

        return AvailabilityResult.Yes();
    }
}
