namespace WeddingApi.core.Interfaces
{
    public interface IUnitOfWorks: IDisposable
    {
        IBookingRepository Bookings { get; }
        IClientRepository Clients { get; }
		IServiceProviderRepository ServiceProviders { get; } // Repository for ServiceProvider entities
		IPaymentRepository Payments { get; }
		Task<int> CompleteAsync();
    }
}
