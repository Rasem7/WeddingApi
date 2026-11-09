using Microsoft.EntityFrameworkCore;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories;

public class TaskItemRepository : ITaskItemRepository
{
    private readonly WeddingDbContext _db;
    public TaskItemRepository(WeddingDbContext db) => _db = db;

    public async Task<TaskItem?> GetByIdAsync(int id) =>
        await _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id);

    public async Task<List<TaskItem>> GetByOwnersAsync(IEnumerable<int> ownerClientIds)
    {
        var ids = ownerClientIds.ToList();
        return await _db.TaskItems
            .Where(t => ids.Contains(t.OwnerClientId))
            .ToListAsync();
    }

    public async Task<TaskItem> CreateAsync(TaskItem item)
    {
        _db.TaskItems.Add(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task<TaskItem> UpdateAsync(TaskItem item)
    {
        _db.TaskItems.Update(item);
        await _db.SaveChangesAsync();
        return item;
    }

    public async Task DeleteAsync(int id)
    {
        var item = await _db.TaskItems.FindAsync(id);
        if (item != null)
        {
            _db.TaskItems.Remove(item);
            await _db.SaveChangesAsync();
        }
    }
}
