using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<TaskItem?> GetByIdAsync(int id);
        // كل المهام اللي أصحابها ضمن المجموعة دي (العميل + شريكه المربوط)
        Task<List<TaskItem>> GetByOwnersAsync(IEnumerable<int> ownerClientIds);
        Task<TaskItem> CreateAsync(TaskItem item);
        Task<TaskItem> UpdateAsync(TaskItem item);
        Task DeleteAsync(int id);
    }
}
