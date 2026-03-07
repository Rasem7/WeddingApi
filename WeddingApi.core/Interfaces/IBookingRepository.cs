using WeddingApi.core.Common;
using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IBookingRepository
    {
        Task<PagedResult<Booking>> GetAllAsync(QueryParams query);
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetByClientIdAsync(int clientId);
        Task<List<Booking>> GetCalendarAsync(int year, int month);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateStatusAsync(int id, string status);
    }
}
