using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IPaymentRepository
    {
        Task<Payment> GetByIdAsync(int id);
        Task<List<Payment>> GetByBookingIdAsync(int bookingId);
        Task<List<Payment>> GetAllAsync();
        Task<decimal> GetTotalPaidForBookingAsync(int bookingId);
        Task<Payment> CreateAsync(Payment payment);
        Task DeleteAsync(int id);
    }
}
