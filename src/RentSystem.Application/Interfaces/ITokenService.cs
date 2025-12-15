using RentSystem.Domain.Entities;

namespace RentSystem.Application.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        Task<string> StoreRefreshTokenAsync(int userId, string token);
        Task<bool> ValidateRefreshTokenAsync(string token);
        Task<User?> GetUserFromRefreshTokenAsync(string token);
    }
}
