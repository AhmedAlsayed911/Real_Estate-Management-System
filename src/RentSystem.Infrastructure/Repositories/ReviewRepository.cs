using Microsoft.EntityFrameworkCore;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;
using RentSystem.Infrastructure.Data;

namespace RentSystem.Infrastructure.Repositories
{
    public class ReviewRepository : Repository<Review>, IReviewRepository
    {
        public ReviewRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Review>> GetReviewsByPropertyAsync(int propertyId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Property)
                .Where(r => r.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId)
        {
            return await _dbSet
                .Include(r => r.User)
                .Include(r => r.Property)
                .Where(r => r.UserId == userId)
                .ToListAsync();
        }
    }
}
