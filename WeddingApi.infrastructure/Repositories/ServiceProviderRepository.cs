using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeddingApi.core.Entities;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;

namespace WeddingApi.infrastructure.Repositories
{
	public class ServiceProviderRepository : IServiceProviderRepository
	{
		private readonly WeddingDbContext _db;	// Our DbContext to access the database

		public ServiceProviderRepository(WeddingDbContext db)
		{
			_db = db;	// inject DbContext
		}

		public async Task<ServiceProvider?> GetByIdAsync(int id)
		{
			return await _db.ServiceProviders.FindAsync(id); // Find entity by primary key
		}

		public async Task<IEnumerable<ServiceProvider>> GetActiveByCategoryAsync(string? category)
		{
			var query = _db.ServiceProviders.Where(sp => sp.IsActive); // Only active service providers
			if (!string.IsNullOrEmpty(category) && category != "الكل") // Filter by category if not "الكل"
				query = query.Where(sp => sp.Category == category);

			return await query.OrderByDescending(sp => sp.Rating).ToListAsync();  // Sort by rating desc
		}

		public async Task AddAsync(ServiceProvider sp)
		{
			await _db.ServiceProviders.AddAsync(sp); // Add new entity to DbSet
		}

		public void Update(ServiceProvider sp)
		{
			_db.ServiceProviders.Update(sp);  // Mark entity as modified
		}
	}
}
