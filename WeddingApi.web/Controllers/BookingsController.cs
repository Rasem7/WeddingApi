using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IBookingRepository _repo;
    public BookingsController(IBookingRepository repo) => _repo = repo;

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] QueryParams q)
        => Ok(await _repo.GetAllAsync(q));

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _repo.GetByIdAsync(id);
        if (booking == null) return NotFound();
        return Ok(booking);
    }

    [HttpGet("calendar")]
    public async Task<IActionResult> GetCalendar([FromQuery] int year, [FromQuery] int month)
        => Ok(await _repo.GetCalendarAsync(year, month));

    [HttpPost]
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
        var created = await _repo.CreateAsync(booking);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPatch("{id}/status")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var updated = await _repo.UpdateStatusAsync(id, status);
        return Ok(updated);
    }
}