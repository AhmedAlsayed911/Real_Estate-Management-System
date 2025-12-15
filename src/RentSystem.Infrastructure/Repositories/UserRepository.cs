using Microsoft.EntityFrameworkCore;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;
using RentSystem.Infrastructure.Data;

namespace RentSystem.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetUserWithPropertiesAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Properties)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithBookingsAsync(int userId)
        {
            return await _dbSet
                .Include(u => u.Bookings)
                    .ThenInclude(b => b.Property)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}
