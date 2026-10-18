using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task<ProviderBlockedDate?> GetByIdAsync(int id);
        Task<List<ProviderBlockedDate>> GetBlockedAsync(int serviceProviderId, DateTime from, DateTime to);
        Task<bool> IsBlockedAsync(int serviceProviderId, DateTime date);
        Task AddRangeAsync(IEnumerable<ProviderBlockedDate> items);
        Task DeleteAsync(int id);
    }
}
