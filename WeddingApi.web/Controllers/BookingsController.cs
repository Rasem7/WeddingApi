using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Common;
using WeddingApi.core.DTOs.Bookings;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.web.Controllers;

[ApiController]
[Route("api/[controller]")]
//[Authorize]
public class BookingsController : ControllerBase
{
    private readonly IBookingRepository _repo;
    private readonly WeddingDbContext _Context;
   
    public BookingsController(WeddingDbContext Context , IBookingRepository repo)
    {
        _Context = Context;
        _repo = repo;
    }
   
    [HttpGet(nameof(GetAll))]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAll(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchText = null,
        string? status = null,
        string? eventType = null)
    {
        var query = _Context.Bookings.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            query = query.Where(x =>
                (x.Status ?? "").Contains(searchText.Trim()) || (x.Venue ?? "").Contains(searchText.Trim()) ||
                (x.EventType ?? "").Contains(searchText.Trim()));  
               
        }
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = query.Where(x => x.EventType == eventType);
        }

        var bookings = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        if (!bookings.Any())
            return NotFound(); 

        return Ok(bookings); 
    }
    
    
    [HttpGet(nameof(GetAllWithoutPaging))]
    public async Task<ActionResult<IEnumerable<Booking>>> GetAllWithoutPaging(
      string? searchText = null,
      string? status = null,
      string? eventType = null)
    {
        var query = _Context.Bookings.AsQueryable();

        // 🔍 Search
        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var search = searchText.Trim();

            query = query.Where(x =>
                (x.Status ?? "").Contains(search) ||
                (x.Venue ?? "").Contains(search) ||
                (x.EventType ?? "").Contains(search)
            );
        }

        // 🎯 Filter by Status
        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(x => x.Status == status);
        }

        // 🎯 Filter by Event Type
        if (!string.IsNullOrWhiteSpace(eventType))
        {
            query = query.Where(x => x.EventType == eventType);
        }

        var bookings = await query
            .OrderByDescending(x => x.Id)
            .ToListAsync(); // ✅ شيلنا Skip & Take

        if (!bookings.Any())
            return NotFound();

        return Ok(bookings);
    }

    //[HttpGet(nameof(GetById))]
    //public async Task<IActionResult> GetById(int id)
    //{
    //    var booking = await _repo.GetByIdAsync(id);
    //    if (booking == null) return NotFound();
    //    return Ok(booking);
    //}

    [HttpGet(nameof(GetById))]
    public async Task<IActionResult> GetById(int id)
    {
        var booking = await _repo.GetByIdAsync(id);
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
        var bookings = await _repo.GetCalendarAsync(year, month);

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

    [HttpPost(nameof(UpdateStatus))]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] string status)
    {
        var updated = await _repo.UpdateStatusAsync(id, status);
        return Ok(updated);
    }
}