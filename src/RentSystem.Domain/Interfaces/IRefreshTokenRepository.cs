using RentSystem.Domain.Entities;

namespace RentSystem.Domain.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token);
        Task<IEnumerable<RefreshToken>> GetActiveTokensByUserIdAsync(int userId);
        Task RevokeTokenAsync(string token);
        Task RevokeAllUserTokensAsync(int userId);
    }
}
