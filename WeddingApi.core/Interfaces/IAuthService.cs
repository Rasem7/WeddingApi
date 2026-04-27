using WeddingApi.core.DTOs.Auth;

namespace WeddingApi.core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> LoginAsync(LoginDto dto);
        /*Task<object> GetProfileAsync(int userId);
        Task<object> UpdateProfileAsync(int userId, UpdateProfileDto dto);
        Task ChangePasswordAsync(int userId, ChangePasswordDto dto);
        Task<List<object>> GetPendingProvidersAsync();*/
    }
}
