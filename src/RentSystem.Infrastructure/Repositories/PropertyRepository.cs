using Microsoft.EntityFrameworkCore;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;
using RentSystem.Infrastructure.Data;

namespace RentSystem.Infrastructure.Repositories
{
    public class PropertyRepository : Repository<Property>, IPropertyRepository
    {
        public PropertyRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Property?> GetPropertyWithDetailsAsync(int propertyId)
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Include(p => p.Reviews)
                .Include(p => p.Bookings)
                .FirstOrDefaultAsync(p => p.Id == propertyId);
        }

        public async Task<IEnumerable<Property>> GetPropertiesByOwnerAsync(int ownerId)
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Property>> GetPropertiesByLocationAsync(string location)
        {
            return await _dbSet
                .Include(p => p.Owner)
                .Where(p => p.Location.Contains(location))
                .ToListAsync();
        }
    }
}
