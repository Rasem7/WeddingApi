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
        var query = _db.Bookings.Include(b => b.Client).Include(b => b.Service).AsQueryable();

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
        await _db.Bookings.Include(b => b.Client).Include(b => b.Service).Include(b => b.Payments)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<PagedResult<Booking>> SearchAsync(int pageNumber, int pageSize, string? searchText, string? status, string? eventType)
    {
        var query = _db.Bookings.Include(b => b.Client).Include(b => b.Service).AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchText))
        {
            var term = searchText.Trim();
            query = query.Where(x =>
                (x.Status ?? "").Contains(term) ||
                (x.Venue ?? "").Contains(term) ||
                (x.EventType ?? "").Contains(term));
        }
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);
        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(x => x.EventType == eventType);

        var total = await query.CountAsync();
        var data = await query
            .OrderByDescending(x => x.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Booking>
        {
            Data = data,
            TotalCount = total,
            Page = pageNumber,
            PageSize = pageSize
        };
    }

    public async Task<List<Booking>> GetAllWithClientAsync() =>
        await _db.Bookings.Include(b => b.Client).Include(b => b.Service)
            .OrderByDescending(x => x.Id)
            .ToListAsync();

    public async Task<int> CountAllAsync() => await _db.Bookings.CountAsync();

    public async Task<int> CountSinceAsync(DateTime since) =>
        await _db.Bookings.CountAsync(b => b.CreatedAt >= since);

    public async Task<List<Booking>> GetRecentWithClientAsync(int take) =>
        await _db.Bookings.Include(b => b.Client)
            .OrderByDescending(b => b.CreatedAt)
            .Take(take)
            .ToListAsync();

    public async Task<List<Booking>> GetByClientIdAsync(int clientId) =>
        await _db.Bookings.Include(b => b.Client).Include(b => b.Service)
            .Where(b => b.ClientId == clientId)
            .OrderByDescending(b => b.WeddingDate).ToListAsync();

    public async Task<List<Booking>> GetByProviderIdAsync(int serviceProviderId) =>
        await _db.Bookings.Include(b => b.Client).Include(b => b.Service)
            .Where(b => b.Service != null && b.Service.ServiceProviderId == serviceProviderId)
            .OrderByDescending(b => b.WeddingDate).ToListAsync();

    // الحجوزات اللي بتحجز اليوم فعليًا: Pending و Confirmed بس.
    public async Task<int> CountActiveForProviderOnDateAsync(int serviceProviderId, DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);
        return await _db.Bookings.CountAsync(b =>
            b.Service != null && b.Service.ServiceProviderId == serviceProviderId
            && b.WeddingDate >= start && b.WeddingDate < end
            && (b.Status == "Pending" || b.Status == "Confirmed"));
    }

    public async Task<List<DateTime>> GetActiveBookingDatesAsync(int serviceProviderId, DateTime from, DateTime to)
    {
        var start = from.Date;
        var end = to.Date.AddDays(1);
        var dates = await _db.Bookings
            .Where(b => b.Service != null && b.Service.ServiceProviderId == serviceProviderId
                && b.WeddingDate >= start && b.WeddingDate < end
                && (b.Status == "Pending" || b.Status == "Confirmed"))
            .Select(b => b.WeddingDate)
            .ToListAsync();

        return dates.Select(d => d.Date).ToList();
    }

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