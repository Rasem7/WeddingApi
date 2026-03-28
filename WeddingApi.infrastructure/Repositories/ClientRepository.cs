using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Common;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class ClientRepository : IClientRepository
{
    private readonly WeddingDbContext _db;
    public ClientRepository(WeddingDbContext db) => _db = db;

    public async Task<PagedResult<Client>> GetAllAsync(QueryParams q)
    {
        var query = _db.Clients.AsQueryable();

        if (!string.IsNullOrEmpty(q.Search))
            query = query.Where(c =>
                c.GroomName.Contains(q.Search) ||
                c.BrideName.Contains(q.Search) ||
                c.GroomPhone.Contains(q.Search));

        if (!string.IsNullOrEmpty(q.BudgetCategory))
            query = query.Where(c => c.BudgetCategory == q.BudgetCategory);

        var total = await query.CountAsync();
        var data = await query
            .OrderByDescending(c => c.CreatedAt)
            .Skip((q.Page - 1) * q.PageSize)
            .Take(q.PageSize)
            .ToListAsync();

        return new PagedResult<Client>
        {
            Data = data,
            TotalCount = total,
            Page = q.Page,
            PageSize = q.PageSize
        };
    }

    public async Task<Client?> GetByIdAsync(int id) =>
        await _db.Clients.Include(c => c.Bookings).FirstOrDefaultAsync(c => c.Id == id);

    public async Task<Client> CreateAsync(Client client)
    {
        // Auto budget category
        client.BudgetCategory = client.Budget switch
        {
            < 30000 => "Economical",
            < 70000 => "Standard",
            < 150000 => "Premium",
            _ => "Luxury"
        };
        _db.Clients.Add(client);
        await _db.SaveChangesAsync();
        return client;
    }

    public async Task<Client> UpdateAsync(Client client)
    {
        client.BudgetCategory = client.Budget switch
        {
            < 30000 => "Economical",
            < 70000 => "Standard",
            < 150000 => "Premium",
            _ => "Luxury"
        };
        _db.Clients.Update(client);
        await _db.SaveChangesAsync();
        return client;
    }

    public async Task DeleteAsync(int id)
    {
        var client = await _db.Clients.FindAsync(id);
        if (client != null)
        {
            _db.Clients.Remove(client);
            await _db.SaveChangesAsync();
        }
    }
}