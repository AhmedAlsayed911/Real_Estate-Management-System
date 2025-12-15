using RentSystem.Application.DTOs;

namespace RentSystem.Application.Interfaces
{
    public interface IBookingService
    {
        Task<BookingDetailsDto?> GetBookingByIdAsync(int id);
        Task<IEnumerable<BookingDetailsDto>> GetBookingsByPropertyAsync(int propertyId);
        Task<IEnumerable<BookingDetailsDto>> GetBookingsByRenterAsync(int renterId);
        Task<BookingDetailsDto> CreateBookingAsync(int renterId, BookingDto dto);
        Task<BookingDetailsDto?> UpdateBookingAsync(int id, BookingUpdateDto dto);
        Task<bool> DeleteBookingAsync(int id);
    }
}
