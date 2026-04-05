using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IUnitOfWorks: IDisposable
    {
        IBookingRepository Bookings { get; }
        IClientRepository Clients { get; }
		IServiceProviderRepository ServiceProviders { get; } // Repository for ServiceProvider entities
		Task<int> CompleteAsync();
    }
}
