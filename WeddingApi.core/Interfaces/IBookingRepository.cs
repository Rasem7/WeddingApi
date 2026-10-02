using WeddingApi.core.Common;
using WeddingApi.core.Entities;

namespace WeddingApi.core.Interfaces
{
    public interface IBookingRepository
    {
        Task<PagedResult<Booking>> GetAllAsync(QueryParams query);
        Task<PagedResult<Booking>> SearchAsync(int pageNumber, int pageSize, string? searchText, string? status, string? eventType);
        Task<List<Booking>> GetAllWithClientAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task<List<Booking>> GetByClientIdAsync(int clientId);
        Task<List<Booking>> GetCalendarAsync(int year, int month);
        Task<Booking> CreateAsync(Booking booking);
        Task<Booking> UpdateStatusAsync(int id, string status);
        Task<int> CountAllAsync();
        Task<int> CountSinceAsync(DateTime since);
        Task<List<Booking>> GetRecentWithClientAsync(int take);
    }
}
