using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
	public interface IServiceProviderRepository
	{
		Task<ServiceProvider?> GetByIdAsync(int id);
		Task<IEnumerable<ServiceProvider>> GetActiveByCategoryAsync(string? category);
		Task AddAsync(ServiceProvider sp);
		void Update(ServiceProvider sp);
	}
}
