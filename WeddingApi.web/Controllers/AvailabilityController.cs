using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Availability;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AvailabilityController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;
    private readonly IAvailabilityService _availability;

    public AvailabilityController(IUnitOfWorks unitOfWork, IAvailabilityService availability)
    {
        _unitOfWork = unitOfWork;
        _availability = availability;
    }

    private bool IsStaff => User.IsInRole("Admin") || User.IsInRole("Supervisor");

    // Id بتاع المزود = Id بتاع اليوزر بتاعه (Shared Key).
    private bool IsOwner(int providerId) =>
        int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var me) && me == providerId;

    // GET: api/Availability/GetByProvider?serviceProviderId=5&from=2027-06-01&to=2027-08-31
    // عامة — بترجع الأيام غير المتاحة بس (مقفولة أو اكتملت سعتها)، من غير أي بيانات عملاء.
    [HttpGet(nameof(GetByProvider))]
    public async Task<IActionResult> GetByProvider(int serviceProviderId, DateTime? from = null, DateTime? to = null)
    {
        var provider = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId);
        if (provider == null) return NotFound();

        var canSeeDetails = IsStaff || IsOwner(serviceProviderId);
        if (provider.ProviderStatus != "approved" && !canSeeDetails) return NotFound();

        var start = (from ?? DateTime.UtcNow).Date;
        var end = (to ?? start.AddDays(90)).Date;

        if (end < start)
            return BadRequest(new { message = "تاريخ النهاية لازم يكون بعد تاريخ البداية." });
        if ((end - start).TotalDays > 366)
            return BadRequest(new { message = "الفترة المطلوبة لا تزيد عن 366 يوم." });

        var days = new List<AvailabilityDayDto>();

        var blocked = await _unitOfWork.Availability.GetBlockedAsync(serviceProviderId, start, end);
        foreach (var b in blocked)
        {
            days.Add(new AvailabilityDayDto
            {
                Date = b.Date.Date,
                Status = "Blocked",
                Id = canSeeDetails ? b.Id : null,
                Reason = canSeeDetails ? b.Reason : null
            });
        }

        var capacity = AvailabilityPolicy.DailyCapacity(provider.Category);
        if (capacity.HasValue)
        {
            var blockedDays = blocked.Select(b => b.Date.Date).ToHashSet();
            var bookedDates = await _unitOfWork.Bookings.GetActiveBookingDatesAsync(serviceProviderId, start, end);

            foreach (var group in bookedDates.GroupBy(d => d.Date))
            {
                if (group.Count() >= capacity.Value && !blockedDays.Contains(group.Key))
                    days.Add(new AvailabilityDayDto { Date = group.Key, Status = "Booked" });
            }
        }

        return Ok(new
        {
            serviceProviderId,
            from = start,
            to = end,
            unavailableDays = days.OrderBy(d => d.Date)
        });
    }

    // GET: api/Availability/Check?serviceId=3&date=2027-06-15
    // هل الباقة دي متاحة في اليوم ده؟
    [HttpGet(nameof(Check))]
    public async Task<IActionResult> Check(int serviceId, DateTime date)
    {
        var service = await _unitOfWork.Services.GetByIdAsync(serviceId);
        if (service == null || service.ServiceProvider == null)
            return NotFound(new { message = "الباقة غير موجودة." });

        if (!service.IsActive || !service.ServiceProvider.IsActive || service.ServiceProvider.ProviderStatus != "approved")
            return Ok(AvailabilityResult.No("هذه الباقة غير متاحة للحجز حاليًا."));

        var result = await _availability.CheckAsync(service.ServiceProviderId, service.ServiceProvider.Category, date);
        return Ok(result);
    }

    // POST: api/Availability/Block
    // المزود يقفل يوم أو فترة (من StartDate لحد EndDate). الأيام اللي عليها حجوزات فعلية
    // بتتخطى (مش بتتقفل) وبترجع في skippedBecauseBooked عشان تتعامل معاها يدويًا.
    [HttpPost(nameof(Block))]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Block(BlockDatesDto dto)
    {
        if (!IsStaff && !IsOwner(dto.ServiceProviderId))
            return Forbid();

        var provider = await _unitOfWork.ServiceProviders.GetByIdAsync(dto.ServiceProviderId);
        if (provider == null)
            return NotFound(new { message = "مزود الخدمة غير موجود." });

        var start = dto.StartDate.Date;
        var end = (dto.EndDate ?? dto.StartDate).Date;

        if (end < start)
            return BadRequest(new { message = "تاريخ النهاية لازم يكون بعد تاريخ البداية." });
        if (start < DateTime.UtcNow.Date)
            return BadRequest(new { message = "مينفعش تقفل أيام في الماضي." });
        if ((end - start).TotalDays > 365)
            return BadRequest(new { message = "أقصى فترة للقفل مرة واحدة 365 يوم." });

        var alreadyBlocked = (await _unitOfWork.Availability.GetBlockedAsync(dto.ServiceProviderId, start, end))
            .Select(b => b.Date.Date).ToHashSet();
        var bookedDays = (await _unitOfWork.Bookings.GetActiveBookingDatesAsync(dto.ServiceProviderId, start, end))
            .Select(d => d.Date).ToHashSet();

        var toAdd = new List<ProviderBlockedDate>();
        var skipped = new List<string>();

        for (var day = start; day <= end; day = day.AddDays(1))
        {
            if (alreadyBlocked.Contains(day)) continue;

            if (bookedDays.Contains(day))
            {
                skipped.Add(day.ToString("yyyy-MM-dd"));
                continue;
            }

            toAdd.Add(new ProviderBlockedDate
            {
                ServiceProviderId = dto.ServiceProviderId,
                Date = day,
                Reason = dto.Reason ?? string.Empty
            });
        }

        if (toAdd.Count > 0)
            await _unitOfWork.Availability.AddRangeAsync(toAdd);

        return Ok(new
        {
            blockedDays = toAdd.Count,
            alreadyBlockedDays = alreadyBlocked.Count,
            skippedBecauseBooked = skipped
        });
    }

    // POST: api/Availability/Unblock?id=12
    // الـ id بيجي من GetByProvider (للمزود صاحب الحساب والأدمن/المشرف بس).
    [HttpPost(nameof(Unblock))]
    [Authorize(Roles = "Provider,Admin,Supervisor")]
    public async Task<IActionResult> Unblock(int id)
    {
        var item = await _unitOfWork.Availability.GetByIdAsync(id);
        if (item == null) return NotFound();

        if (!IsStaff && !IsOwner(item.ServiceProviderId))
            return Forbid();

        await _unitOfWork.Availability.DeleteAsync(id);
        return Ok(new { message = "تم فتح اليوم", unblockedDate = item.Date.ToString("yyyy-MM-dd") });
    }
}
