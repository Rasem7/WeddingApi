using WeddingApi.core.Common;
using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IClientRepository
    {
        Task<PagedResult<Client>> GetAllAsync(QueryParams query);
        Task<Client?> GetByIdAsync(int id);
        Task<Client> CreateAsync(Client client);
        Task<Client> UpdateAsync(Client client);
        Task DeleteAsync(int id);
    }
}
