using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeddingApi.core.Interfaces;
using WeddingApi.infrastructure.Data;
using WeddingApi.infrastructure.Repositories;

namespace WeddingApi.infrastructure.UnitOfWorks
{
    public class UnitOfWork : IUnitOfWorks
    {
        private readonly WeddingDbContext _context;

		// Private fields for lazy initialization
		private IBookingRepository _bookingRepository;
        private IClientRepository _clientRepository;
        private IServiceProviderRepository _serviceProviderRepository;


		public UnitOfWork(WeddingDbContext context)
        {
            _context = context;
        }

        public IBookingRepository Bookings
        {
            get
            {
                if (_bookingRepository == null)
                    _bookingRepository = new BookingRepository(_context);
                return _bookingRepository;
            }
        }

        public IClientRepository Clients
        {
            get
            {
                if (_clientRepository == null)
                    _clientRepository = new ClientRepository(_context);
                return _clientRepository;
            }
        }

		public IServiceProviderRepository ServiceProviders
        {
			get
			{
				if (_serviceProviderRepository == null)
					_serviceProviderRepository = new ServiceProviderRepository(_context); // Lazy init
				return _serviceProviderRepository;
			}
		}

		public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync(); // Save all changes in one transaction
		}

        public void Dispose()
        {
            _context.Dispose(); // Close DbContext to release DB connections
		}
    }
}