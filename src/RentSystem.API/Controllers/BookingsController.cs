using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentSystem.Application.DTOs;
using RentSystem.Application.Interfaces;
using System.Security.Claims;

namespace RentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBookingById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            return Ok(booking);
        }

        [HttpGet("property/{propertyId}")]
        [Authorize(Roles = "owner,admin")]
        public async Task<IActionResult> GetBookingsByProperty(int propertyId)
        {
            var bookings = await _bookingService.GetBookingsByPropertyAsync(propertyId);
            return Ok(bookings);
        }

        [HttpGet("renter/{renterId}")]
        public async Task<IActionResult> GetBookingsByRenter(int renterId)
        {
            var bookings = await _bookingService.GetBookingsByRenterAsync(renterId);
            return Ok(bookings);
        }

        [HttpGet("my-bookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var bookings = await _bookingService.GetBookingsByRenterAsync(userId);
            return Ok(bookings);
        }

        [HttpPost]
        [Authorize(Roles = "renter,admin")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var booking = await _bookingService.CreateBookingAsync(userId, dto);

            return CreatedAtAction(nameof(GetBookingById), new { id = booking.Id }, booking);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto dto)
        {
            var booking = await _bookingService.UpdateBookingAsync(id, dto);

            if (booking == null)
            {
                return NotFound(new { message = "Booking not found" });
            }

            return Ok(booking);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var result = await _bookingService.DeleteBookingAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Booking not found" });
            }

            return NoContent();
        }
    }
}
