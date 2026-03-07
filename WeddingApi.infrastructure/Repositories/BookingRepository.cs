using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Common;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class BookingRepository : IBookingRepository
{
    private readonly WeddingDbContext _db;
    public BookingRepository(WeddingDbContext db) => _db = db;

    public async Task<PagedResult<Booking>> GetAllAsync(QueryParams q)
    {
        var query = _db.Bookings.Include(b => b.Client).AsQueryable();

        if (!string.IsNullOrEmpty(q.Status))
            query = query.Where(b => b.Status == q.Status);

        var total = await query.CountAsync();
        var data = await query
            .OrderByDescending(b => b.WeddingDate)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();

        return new PagedResult<Booking>
        {
            Data = data,
            TotalCount = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

    public async Task<Booking?> GetByIdAsync(int id) =>
        await _db.Bookings.Include(b => b.Client).Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<List<Booking>> GetByClientIdAsync(int clientId) =>
        await _db.Bookings.Where(b => b.ClientId == clientId)
            .OrderByDescending(b => b.WeddingDate).ToListAsync();

    public async Task<List<Booking>> GetCalendarAsync(int year, int month) =>
        await _db.Bookings.Include(b => b.Client)
            .Where(b => b.WeddingDate.Year == year && b.WeddingDate.Month == month)
            .ToListAsync();

    public async Task<Booking> CreateAsync(Booking booking)
    {
        _db.Bookings.Add(booking);
        await _db.SaveChangesAsync();
        return booking;
    }

    public async Task<Booking> UpdateStatusAsync(int id, string status)
    {
        var booking = await _db.Bookings.FindAsync(id)
            ?? throw new Exception("Booking not found");
        booking.Status = status;
        await _db.SaveChangesAsync();
        return booking;
    }
}