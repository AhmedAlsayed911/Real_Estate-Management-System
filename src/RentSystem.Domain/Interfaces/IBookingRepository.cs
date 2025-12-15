using RentSystem.Domain.Entities;

namespace RentSystem.Domain.Interfaces
{
    public interface IBookingRepository : IRepository<Booking>
    {
        Task<IEnumerable<Booking>> GetBookingsByPropertyAsync(int propertyId);
        Task<IEnumerable<Booking>> GetBookingsByRenterAsync(int renterId);
        Task<Booking?> GetBookingWithDetailsAsync(int bookingId);
    }
}
