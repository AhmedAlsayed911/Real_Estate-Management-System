using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApi.Dtos;
using RentApi.Helpers;
using RentApi.Models;
using System.Security.Claims;

namespace RentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PropertyController(ApplicationDbContext dbContext, Attachments attachments) : ControllerBase
    {
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAllAsync()
        {
            var properties = await dbContext.Set<Property>().ToListAsync();
            return properties.Count is 0 ? NotFound("No Properties Found!") : Ok(properties);
        }

        [HttpGet("GetAllIncludeDetails")]
        public async Task<IActionResult> GetAllInclude()
        {
            var properties = await dbContext.Set<Property>()
                                            .Include(p => p.Owner)
                                            .Include(p => p.Bookings)
                                            .Include(p => p.Reviews)
                                            .Select(p => new PropertyDetailsDto
                                            {
                                                Id = p.Id,
                                                Title = p.Title,
                                                PricePerNight = p.PricePerNight,
                                                Location = p.Location,
                                                OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                                                BookingCount = p.Bookings.Count,
                                                AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0
                                            }).ToListAsync();

            return properties.Count == 0 ? NotFound("No Properties Found!") : Ok(properties);
        }

        [HttpGet("AllWithUserReview")]
        [Authorize]
        public async Task<IActionResult> GetAllWithUserReview()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var properties = await dbContext.Set<Property>()
                .Include(p => p.Reviews)
                .Include(p => p.Owner)
                .Select(p => new
                {
                    Property = new
                    {
                        p.Id,
                        p.Title,
                        p.Location,
                        p.PricePerNight,
                        p.Description,
                        MainImageUrl = p.MainImageUrl != null ? Convert.ToBase64String(p.MainImageUrl) : null,
                        OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                        AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0,
                        TotalReviews = p.Reviews.Count                       
                    },
                    UserReview = p.Reviews
                        .Where(r => r.UserId == userId)
                        .Select(r => new
                        {
                            r.Id,
                            r.Rating,
                            r.Comment
                        })
                        .FirstOrDefault()
                })
                .ToListAsync();

            return Ok(properties);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var property = await dbContext.Set<Property>().FindAsync(id);
            return property is null ? NotFound("No Property Found") : Ok(property.Description);
        }

        [HttpGet("GetByIdWithDetails/{id}")]
        public async Task<IActionResult> GetByIdWithDetails(int id)
        {
            var property = await dbContext.Set<Property>()
                .Include(p => p.Owner)
                .Include(p => p.Bookings)
                .Include(p => p.Reviews)
                .Where(p => p.Id == id)
                .Select(p => new PropertyDetailsDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    PricePerNight = p.PricePerNight,
                    Location = p.Location,
                    OwnerName = p.Owner.FirstName + " " + p.Owner.LastName,
                    BookingCount = p.Bookings.Count,
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0
                })
                .SingleOrDefaultAsync();

            return property is null ? NotFound("No Property Found!") : Ok(property);
        }

        [HttpGet("GetByTitle")]
        public IActionResult GetByTitle(string title)
        {
            var property = dbContext.Set<Property>().SingleOrDefault(x => x.Title.ToLower() == title.ToLower());
            return property is null ? NotFound("No Properties Found!") : Ok(property);
        }

        [HttpGet("MyProperties")]
        [Authorize]
        public async Task<IActionResult> GetMyProperties()
        {           
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var properties = await dbContext.Set<Property>()
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Bookings)
                .Include(p => p.Reviews)
                .Select(p => new PropertyDetailsDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    PricePerNight = p.PricePerNight,
                    Location = p.Location,
                    MainImageUrl = p.MainImageUrl != null ? Convert.ToBase64String(p.MainImageUrl) : null,
                    BookingCount = p.Bookings.Count,
                    AverageRating = p.Reviews.Any() ? p.Reviews.Average(r => (double)r.Rating) : 0
                })
                .ToListAsync();

            return Ok(properties);
        }

        [HttpGet("Stats")]
        [Authorize(Roles = "owner")]
        public async Task<IActionResult> GetOwnerStats()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var properties = await dbContext.Set<Property>()
                .Where(p => p.OwnerId == userId)
                .Include(p => p.Bookings)
                .Include(p => p.Reviews)
                .ToListAsync();

            var totalProperties = properties.Count;
            var totalBookings = properties.Sum(p => p.Bookings.Count);
            var totalReviews = properties.Sum(p => p.Reviews.Count);

            return Ok(new
            {
                totalProperties,
                totalBookings,
                totalReviews
            });
        }

        [HttpPost("AddProperty")]
        [Authorize]
        public async Task<IActionResult> AddProperty([FromForm] PropertyDto dto)
        {
            if (dto.MainImageUrl is null)
                return BadRequest("Property Image is Required!");

            if (!attachments.AllowedExtensions.Contains(Path.GetExtension(dto.MainImageUrl.FileName).ToLower()))
                return BadRequest("Only .png, .jpg, .jpeg images are allowed!");

            if (dto.MainImageUrl.Length > attachments.MaxByteLength)
                return BadRequest("Max Allowed Size for Image is 1MB!");

            var ownerId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var isValidOwner = await dbContext.Set<User>().FindAsync(ownerId);
            //if (isValidOwner is null || !isValidOwner.Role.Contains("owner", StringComparer.OrdinalIgnoreCase))
            //    return BadRequest("Invalid Owner ID");

            using var dataStream = new MemoryStream();
            await dto.MainImageUrl.CopyToAsync(dataStream);

            var property = new Property
            {
                OwnerId = ownerId,
                Title = dto.Title,
                Description = dto.Description,
                PricePerNight = dto.PricePerNight,
                MainImageUrl = dataStream.ToArray(),
                Location = dto.Location,
            };

            await dbContext.Set<Property>().AddAsync(property);
            await dbContext.SaveChangesAsync();

            return Ok(property);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateProperty(int id, [FromForm] PropertyDtoUpdate dto)
        {
            var property = await dbContext.Set<Property>().FindAsync(id);
            if (property is null)
                return NotFound("Property not found");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (property.OwnerId != userId)
                return Forbid("You are not the owner of this property.");

            if (!string.IsNullOrEmpty(dto.Title)) property.Title = dto.Title;
            if (!string.IsNullOrEmpty(dto.Description)) property.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.Location)) property.Location = dto.Location;
            if (dto.PricePerNight.HasValue) property.PricePerNight = dto.PricePerNight.Value;

            if (dto.MainImageUrl is not null)
            {
                if (!attachments.AllowedExtensions.Contains(Path.GetExtension(dto.MainImageUrl.FileName).ToLower()))
                    return BadRequest("Invalid image extension.");

                using var dataStream = new MemoryStream();
                await dto.MainImageUrl.CopyToAsync(dataStream);
                property.MainImageUrl = dataStream.ToArray();
            }

            dbContext.Update(property);
            await dbContext.SaveChangesAsync();

            return Ok(property);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var property = await dbContext.Set<Property>().FindAsync(id);
            if (property is null)
                return NotFound("No Property Found!");

            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            if (property.OwnerId != userId)
                return Forbid("You are not the owner of this property.");

            dbContext.Remove(property);
            await dbContext.SaveChangesAsync();

            return Ok("Property deleted successfully.");
        }
    }
}
