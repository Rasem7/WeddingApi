using WeddingApi.core.DTOs.Auth;

namespace WeddingApi.core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto dto);
    }
}
