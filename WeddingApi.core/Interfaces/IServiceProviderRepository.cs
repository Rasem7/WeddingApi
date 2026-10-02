using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
	public interface IServiceProviderRepository
	{
		Task<ServiceProvider> GetByIdAsync(int id);
		Task<IEnumerable<ServiceProvider>> GetActiveByCategoryAsync(string category);
		Task AddAsync(ServiceProvider sp);
		void Update(ServiceProvider sp);
	}
}
