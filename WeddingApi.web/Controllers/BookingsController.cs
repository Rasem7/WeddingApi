using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Bookings;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IUnitOfWorks _unitOfWork;

    public BookingsController(IUnitOfWorks unitOfWorks)
    {
        _unitOfWork = unitOfWorks;
    }

    private static BookingDto ToDto(Booking b) => new()
    {
        Id = b.Id,
        ClientId = b.ClientId,
        GroomName = b.Client?.GroomName ?? "",
        BrideName = b.Client?.BrideName ?? "",
        ServiceId = b.ServiceId,
        ServiceName = b.Service?.Name ?? "",
        ServiceProviderId = b.Service?.ServiceProviderId,
        WeddingDate = b.WeddingDate,
        WeddingTime = b.WeddingTime,
        Venue = b.Venue,
        GuestCount = b.GuestCount,
        EventType = b.EventType,
        Status = b.Status,
        TotalAmount = b.TotalAmount,
        Notes = b.Notes,
        CreatedAt = b.CreatedAt
    };

    [HttpGet(nameof(GetAll))]
    public async Task<IActionResult> GetAll(
        int pageNumber = 1,
        int pageSize = 10,
        string searchText = null,
        string status = null,
        string eventType = null)
    {
        var result = await _unitOfWork.Bookings.SearchAsync(pageNumber, pageSize, searchText, status, eventType);

        if (!result.Data.Any())
            return NotFound();

        var dtoResult = new PagedResult<BookingDto>
        {
            Data = result.Data.Select(ToDto).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize
        };

        return Ok(dtoResult);
    }


    [HttpGet(nameof(GetAllWithoutPaging))]
    public async Task<IActionResult> GetAllWithoutPaging()
    {
        var bookings = await _unitOfWork.Bookings.GetAllWithClientAsync();

        if (!bookings.Any())
            return NotFound();

        return Ok(bookings.Select(ToDto));
    }

    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
        if (booking == null) return NotFound();

        var result = new
        {
            booking.Id,
            booking.Status,
            booking.WeddingDate,
            booking.TotalAmount,
            Service = booking.Service == null ? null : new
            {
                booking.Service.Id,
                booking.Service.Name,
                booking.Service.Price,
                booking.Service.ServiceProviderId
            },
            Client = new
            {
                booking.Client.Id,
                booking.Client.BrideName,
                booking.Client.GroomName
            }
        };

        return Ok(result);
    }

    //[HttpGet(nameof(GetCalendar))]
    //public async Task<IActionResult> GetCalendar([FromQuery] int year, [FromQuery] int month)
    //    => Ok(await _repo.GetCalendarAsync(year, month));

    [HttpGet(nameof(GetCalendar))]
    public async Task<IActionResult> GetCalendar([FromQuery] int year, [FromQuery] int month)
    {
        var bookings = await _unitOfWork.Bookings.GetCalendarAsync(year, month);

        var result = bookings.Select(b => new
        {
            b.Id,
            b.WeddingDate,
            b.Status,
            BrideName = b.Client.BrideName,
            GroomName = b.Client.GroomName
        });

        return Ok(result);
    }
    [HttpPost(nameof(Create))]
    [Authorize(Roles = "Client,Admin,Supervisor")]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var clientId = dto.ClientId;

        // لو اللي بيحجز هو العميل نفسه، نتجاهل أي ClientId جاي في الـ body
        // ونستخدم هويته من التوكن، عشان نمنعه يحجز باسم عميل تاني.
        // الأدمن/المشرف لوحدهم المسموح لهم يحددوا ClientId مختلف (حجز يدوي بالنيابة عن عميل).
        if (User.IsInRole("Client"))
        {
            var tokenUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(tokenUserId, out clientId))
                return Unauthorized(new { message = "تعذر التحقق من هوية العميل من التوكن." });
        }

        // تحديد السعر: من الباقة لو اتحددت، وإلا من المبلغ المُدخل يدويًا.
        decimal totalAmount;
        if (dto.ServiceId.HasValue)
        {
            var service = await _unitOfWork.Services.GetByIdAsync(dto.ServiceId.Value);
            if (service == null)
                return NotFound(new { message = "الباقة المطلوبة غير موجودة." });

            if (!service.IsActive || service.ServiceProvider == null
                || !service.ServiceProvider.IsActive || service.ServiceProvider.ProviderStatus != "approved")
                return BadRequest(new { message = "هذه الباقة غير متاحة للحجز حاليًا." });

            totalAmount = service.Price; // السعر من الباقة دايمًا (نتجاهل أي مبلغ جاي من العميل)
        }
        else
        {
            if (dto.TotalAmount <= 0)
                return BadRequest(new { message = "لازم تختار باقة (ServiceId) أو تحدد مبلغ أكبر من صفر." });

            totalAmount = dto.TotalAmount;
        }

        var booking = new Booking
        {
            ClientId = clientId,
            ServiceId = dto.ServiceId,
            WeddingDate = dto.WeddingDate,
            WeddingTime = dto.WeddingTime,
            Venue = dto.Venue,
            GuestCount = dto.GuestCount,
            EventType = dto.EventType,
            TotalAmount = totalAmount,
            Notes = dto.Notes
        };
        var created = await _unitOfWork.Bookings.CreateAsync(booking);

        // نجيبه تاني بالـ Client والباقة عشان الرد يبقى DTO كامل بدل الـ Entity الخام.
        var full = await _unitOfWork.Bookings.GetByIdAsync(created.Id);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, ToDto(full ?? created));
    }

    [HttpPost(nameof(UpdateStatus))]
    [Authorize(Roles = "Admin,Supervisor,Provider")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var updated = await _unitOfWork.Bookings.UpdateStatusAsync(id, status);
        return Ok(updated);
    }
}