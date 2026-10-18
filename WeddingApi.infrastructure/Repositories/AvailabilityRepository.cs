using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class AvailabilityRepository : IAvailabilityRepository
{
    private readonly WeddingDbContext _db;
    public AvailabilityRepository(WeddingDbContext db) => _db = db;

    public async Task<ProviderBlockedDate?> GetByIdAsync(int id) =>
        await _db.ProviderBlockedDates.FirstOrDefaultAsync(b => b.Id == id);

    public async Task<List<ProviderBlockedDate>> GetBlockedAsync(int serviceProviderId, DateTime from, DateTime to)
    {
        var start = from.Date;
        var end = to.Date;
        return await _db.ProviderBlockedDates
            .Where(b => b.ServiceProviderId == serviceProviderId && b.Date >= start && b.Date <= end)
            .OrderBy(b => b.Date)
            .ToListAsync();
    }

    public async Task<bool> IsBlockedAsync(int serviceProviderId, DateTime date)
    {
        var day = date.Date;
        return await _db.ProviderBlockedDates
            .AnyAsync(b => b.ServiceProviderId == serviceProviderId && b.Date == day);
    }

    public async Task AddRangeAsync(IEnumerable<ProviderBlockedDate> items)
    {
        _db.ProviderBlockedDates.AddRange(items);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _db.ProviderBlockedDates.FindAsync(id);
        if (item != null)
        {
            _db.ProviderBlockedDates.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
