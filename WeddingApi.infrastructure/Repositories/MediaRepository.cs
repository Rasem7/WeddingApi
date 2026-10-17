using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly WeddingDbContext _db;
    public MediaRepository(WeddingDbContext db) => _db = db;

    public async Task<List<ServiceProviderMedia>> GetByProviderIdAsync(int serviceProviderId) =>
        await _db.ServiceProviderMedias
            .Where(m => m.ServiceProviderId == serviceProviderId)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();

    public async Task<ServiceProviderMedia?> GetByIdAsync(int id) =>
        await _db.ServiceProviderMedias.FindAsync(id);

    public async Task<ServiceProviderMedia> CreateAsync(ServiceProviderMedia media)
    {
        _db.ServiceProviderMedias.Add(media);
        await _db.SaveChangesAsync();
        return media;
    }

    public async Task DeleteAsync(int id)
    {
        var media = await _db.ServiceProviderMedias.FindAsync(id);
        if (media != null)
        {
            _db.ServiceProviderMedias.Remove(media);
            await _db.SaveChangesAsync();
        }
    }
}
