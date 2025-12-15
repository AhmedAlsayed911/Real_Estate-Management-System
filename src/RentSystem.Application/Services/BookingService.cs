using RentSystem.Application.DTOs;
using RentSystem.Application.Interfaces;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;

namespace RentSystem.Application.Services
{
    public class BookingService : IBookingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BookingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BookingDetailsDto?> GetBookingByIdAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetBookingWithDetailsAsync(id);
            return booking != null ? MapToDetailsDto(booking) : null;
        }

        public async Task<IEnumerable<BookingDetailsDto>> GetBookingsByPropertyAsync(int propertyId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByPropertyAsync(propertyId);
            return bookings.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<BookingDetailsDto>> GetBookingsByRenterAsync(int renterId)
        {
            var bookings = await _unitOfWork.Bookings.GetBookingsByRenterAsync(renterId);
            return bookings.Select(MapToDetailsDto);
            
        }

        public async Task<BookingDetailsDto> CreateBookingAsync(int renterId, BookingDto dto)
        {
            var booking = new Booking
            {
                PropertyId = dto.PropertyId,
                RenterId = renterId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await _unitOfWork.Bookings.AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();

            return MapToDetailsDto(booking);
        }

        public async Task<BookingDetailsDto?> UpdateBookingAsync(int id, BookingUpdateDto dto)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return null;
            }

            if (dto.StartDate.HasValue)
                booking.StartDate = dto.StartDate.Value;
            
            if (dto.EndDate.HasValue)
                booking.EndDate = dto.EndDate.Value;

            _unitOfWork.Bookings.Update(booking);
            await _unitOfWork.SaveChangesAsync();

            return MapToDetailsDto(booking);
        }

        public async Task<bool> DeleteBookingAsync(int id)
        {
            var booking = await _unitOfWork.Bookings.GetByIdAsync(id);
            if (booking == null)
            {
                return false;
            }

            _unitOfWork.Bookings.Remove(booking);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private BookingDetailsDto MapToDetailsDto(Booking booking)
        {
            return new BookingDetailsDto
            {
                Id = booking.Id,
                StartDate = booking.StartDate,
                EndDate = booking.EndDate,
                PropertyTitle = booking.Property?.Title ?? string.Empty,
                PropertyLocation = booking.Property?.Location ?? string.Empty,
                RenterFullName = booking.Renter != null 
                    ? $"{booking.Renter.FirstName} {booking.Renter.LastName}" 
                    : string.Empty
            };
        }
    }
}
