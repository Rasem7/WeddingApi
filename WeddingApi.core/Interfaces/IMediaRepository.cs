using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IMediaRepository
    {
        Task<List<ServiceProviderMedia>> GetByProviderIdAsync(int serviceProviderId);
        Task<ServiceProviderMedia?> GetByIdAsync(int id);
        Task<ServiceProviderMedia> CreateAsync(ServiceProviderMedia media);
        Task DeleteAsync(int id);
    }
}
