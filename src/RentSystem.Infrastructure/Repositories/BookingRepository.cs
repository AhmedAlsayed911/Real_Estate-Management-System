using Microsoft.EntityFrameworkCore;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;
using RentSystem.Infrastructure.Data;

namespace RentSystem.Infrastructure.Repositories
{
    public class BookingRepository : Repository<Booking>, IBookingRepository
    {
        public BookingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.Renter)
                .Where(b => b.PropertyId == propertyId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByRenterAsync(int renterId)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.Renter)
                .Where(b => b.RenterId == renterId)
                .ToListAsync();
        }

        public async Task<Booking?> GetBookingWithDetailsAsync(int bookingId)
        {
            return await _dbSet
                .Include(b => b.Property)
                .Include(b => b.Renter)
                .FirstOrDefaultAsync(b => b.Id == bookingId);
        }
    }
}
