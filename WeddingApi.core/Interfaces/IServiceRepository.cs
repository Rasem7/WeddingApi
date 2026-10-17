using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IServiceRepository
    {
        Task<Service?> GetByIdAsync(int id);
        Task<List<Service>> GetByProviderIdAsync(int serviceProviderId, bool activeOnly = true);
        Task<Service> CreateAsync(Service service);
        Task<Service> UpdateAsync(Service service);
        Task DeleteAsync(int id);
    }
}
