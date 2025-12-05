using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApi.Dtos;
using RentApi.Models;
using System;
using System.Linq;
using System.Security.Claims;

namespace RentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController(ApplicationDbContext dbContext) : ControllerBase
    {
        [HttpGet]
        [Route("GetAllBookings")]
        public async Task<IActionResult> GetAllAsync()
        {
            var bookings = await dbContext.Set<Booking>().ToListAsync();
            return bookings.Count is 0 ? NotFound("No Bookings Found!") : Ok(bookings);
        }

        [HttpGet]
        [Route("GetAllWithDetails")]
        public async Task<IActionResult> GetAllBookings()
        {
            var bookings = await dbContext.Set<Booking>()
                .Include(b => b.Property)
                .Include(b => b.Renter)
                .Select(b => new BookingDetailsDto
                {
                    Id = b.Id,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PropertyTitle = b.Property.Title,
                    PropertyLocation = b.Property.Location,
                    RenterFullName = b.Renter.FirstName + " " + b.Renter.LastName
                })
                .ToListAsync();
            return bookings.Count is 0 ? BadRequest("No Bookings Found") : Ok(bookings);
        }

        [HttpGet]
        [Route("GetById{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var booking = await dbContext.Set<Booking>()
                .Include(b => b.Property)
                .Include(b => b.Renter)
                .Where(b => b.Id == id)
                .Select(b => new BookingDetailsDto
                {
                    Id = b.Id,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PropertyTitle = b.Property.Title,
                    PropertyLocation = b.Property.Location,
                    RenterFullName = b.Renter.FirstName + " " + b.Renter.LastName
                })
                .SingleOrDefaultAsync();
            return booking is null ? BadRequest("No Bookings Found") : Ok(booking);
        }

        [HttpGet]
        [Authorize]
        [Route("MyBookings")]
        public async Task<IActionResult> GetMyBookings()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var bookings = await dbContext.Set<Booking>()
                .Include(b => b.Property)
                .Where(b => b.RenterId == userId)
                .Select(b => new BookingDetailsDto
                {
                    Id = b.Id,
                    StartDate = b.StartDate,
                    EndDate = b.EndDate,
                    PropertyTitle = b.Property.Title,
                    PropertyLocation = b.Property.Location,
                    RenterFullName = b.Renter.FirstName + " " + b.Renter.LastName
                })
                .OrderByDescending(b => b.StartDate)
                .ToListAsync();

            return bookings.Count is 0 ? Ok(new List<BookingDetailsDto>()) : Ok(bookings);
        }

        [HttpPost]
        [Authorize]
        [Route("MakeBooking")]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto dto)
        {
            if (dto.EndDate <= dto.StartDate)
                return BadRequest("End date must be after start date.");

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var property = await dbContext.Set<Property>().FindAsync(dto.PropertyId);
            if (property is null)
                return NotFound("Property not found.");

            if (property.OwnerId == int.Parse(userId))
                return BadRequest("You cannot book your own property");

            var isBooked = await dbContext.Set<Booking>().AnyAsync(b =>
                b.PropertyId == dto.PropertyId &&
                (
                    (dto.StartDate >= b.StartDate && dto.StartDate < b.EndDate) ||
                    (dto.EndDate > b.StartDate && dto.EndDate <= b.EndDate) ||
                    (dto.StartDate <= b.StartDate && dto.EndDate >= b.EndDate)
                )
            );

            if (isBooked)
                return BadRequest("This property is already booked in the selected date range.");

            var booking = new Booking
            {
                PropertyId = dto.PropertyId,
                RenterId = int.Parse(userId), 
                StartDate = dto.StartDate,
                EndDate = dto.EndDate
            };

            await dbContext.Set<Booking>().AddAsync(booking);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Booking created successfully.",
                BookingId = booking.Id,
                booking.StartDate,
                booking.EndDate
            });
        }


        [HttpPut]
        [Route("UpdateBooking/{id}")]
        public async Task<IActionResult> UpdateBooking(int id, [FromBody] BookingUpdateDto dto)
        {
            var booking = await dbContext.Set<Booking>().FirstOrDefaultAsync(b => b.Id == id);
            if (booking is null)
                return NotFound("Booking not found.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (booking.RenterId != userId)
                return Forbid("You are not the renter of this booking.");

            var newStartDate = dto.StartDate ?? booking.StartDate;
            var newEndDate = dto.EndDate ?? booking.EndDate;
            var newPropertyId = dto.PropertyId ?? booking.PropertyId;

            if (newEndDate <= newStartDate)
                return BadRequest("End date must be after start date.");

            var property = await dbContext.Set<Property>().FindAsync(newPropertyId);
            if (property is null)
                return BadRequest("Invalid PropertyId.");

            var isBooked = await dbContext.Set<Booking>().AnyAsync(b =>
                b.Id != id &&
                b.PropertyId == newPropertyId &&
                (
                    (newStartDate >= b.StartDate && newStartDate < b.EndDate) ||
                    (newEndDate > b.StartDate && newEndDate <= b.EndDate) ||
                    (newStartDate <= b.StartDate && newEndDate >= b.EndDate)
                )
            );

            if (isBooked)
                return BadRequest("This property is already booked in the selected date range.");

            booking.PropertyId = newPropertyId;
            booking.StartDate = newStartDate;
            booking.EndDate = newEndDate;

            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Booking updated successfully.",
                booking.Id,
                booking.PropertyId,
                booking.StartDate,
                booking.EndDate
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            var booking = await dbContext.Set<Booking>().FindAsync(id);
            if (booking is null)
                return NotFound("Booking not found.");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (booking.RenterId != userId)
                return Forbid("You are not authorized to delete this booking.");

            dbContext.Set<Booking>().Remove(booking);
            await dbContext.SaveChangesAsync();

            return Ok(new
            {
                Message = "Booking deleted successfully.",
                BookingId = id
            });
        }


    }
}
