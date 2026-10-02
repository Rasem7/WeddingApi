using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        return Ok(result);
    }


    [HttpGet(nameof(GetAllWithoutPaging))]
    public async Task<IActionResult> GetAllWithoutPaging()
    {
        var bookings = await _unitOfWork.Bookings.GetAllWithClientAsync();

        if (!bookings.Any())
            return NotFound();

        return Ok(bookings);
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
    // ملحوظة: الحجز مبدئيًا مسموح لأي عميل/أدمن/مشرف. التحسين المستقبلي المطلوب:
    // اشتقاق ClientId من التوكن نفسه (User.FindFirstValue) بدل قبوله في الـ Body،
    // عشان نمنع عميل يعمل حجز باسم عميل تاني.
    [HttpPost(nameof(Create))]
    [Authorize(Roles = "Client,Admin,Supervisor")]
    public async Task<IActionResult> Create(CreateBookingDto dto)
    {
        var booking = new Booking
        {
            ClientId = dto.ClientId,
            WeddingDate = dto.WeddingDate,
            WeddingTime = dto.WeddingTime,
            Venue = dto.Venue,
            GuestCount = dto.GuestCount,
            EventType = dto.EventType,
            TotalAmount = dto.TotalAmount,
            Notes = dto.Notes
        };
        var created = await _unitOfWork.Bookings.CreateAsync(booking);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPost(nameof(UpdateStatus))]
    [Authorize(Roles = "Admin,Supervisor,Provider")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var updated = await _unitOfWork.Bookings.UpdateStatusAsync(id, status);
        return Ok(updated);
    }
}