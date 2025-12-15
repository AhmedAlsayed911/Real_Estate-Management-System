using RentSystem.Domain.Entities;

namespace RentSystem.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetUserWithPropertiesAsync(int userId);
        Task<User?> GetUserWithBookingsAsync(int userId);
    }
}
