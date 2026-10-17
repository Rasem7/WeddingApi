using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class ServiceRepository : IServiceRepository
{
    private readonly WeddingDbContext _db;
    public ServiceRepository(WeddingDbContext db) => _db = db;

    public async Task<Service?> GetByIdAsync(int id) =>
        await _db.Services.Include(s => s.ServiceProvider).FirstOrDefaultAsync(s => s.Id == id);

    public async Task<List<Service>> GetByProviderIdAsync(int serviceProviderId, bool activeOnly = true)
    {
        var query = _db.Services.Where(s => s.ServiceProviderId == serviceProviderId);
        if (activeOnly)
            query = query.Where(s => s.IsActive);

        return await query.OrderBy(s => s.Price).ToListAsync();
    }

    public async Task<Service> CreateAsync(Service service)
    {
        _db.Services.Add(service);
        await _db.SaveChangesAsync();
        return service;
    }

    public async Task<Service> UpdateAsync(Service service)
    {
        _db.Services.Update(service);
        await _db.SaveChangesAsync();
        return service;
    }

    public async Task DeleteAsync(int id)
    {
        var service = await _db.Services.FindAsync(id);
        if (service != null)
        {
            _db.Services.Remove(service);
            await _db.SaveChangesAsync();
        }
    }
}
