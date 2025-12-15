using RentSystem.Application.DTOs;

namespace RentSystem.Application.Interfaces
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterDto dto);
        Task<AuthenticationResult> LoginAsync(LoginDto dto);
        Task<AuthenticationResult> RefreshTokenAsync(string refreshToken);
        Task<bool> RevokeTokenAsync(string refreshToken);
    }
}
