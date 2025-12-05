using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApi.Dtos;
using RentApi.Models;
using System.Security.Claims;

namespace RentApi.Controllers
{
    namespace RentApi.Controllers
    {
        [ApiController]
        [Route("api/[controller]")]
        public class ReviewController(ApplicationDbContext dbContext) : ControllerBase
    {
            [HttpGet]
            [Authorize]
            [Route("GetAllReviews")]
            public async Task<IActionResult> GetAllReviews()
            {
                var reviews = await dbContext.Set<Review>()
                    .Include(r => r.User)
                    .Include(r => r.Property)
                    .Select(r => new ReviewDetailsDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        UserName = r.User.FirstName + " " + r.User.LastName,
                        PropertyTitle = r.Property.Title,                     
                    })
                    .ToListAsync();

                if (reviews.Count == 0)
                    return NotFound("No reviews found.");

                return Ok(reviews);
            }


            [HttpPost]
            [Route("AddReview")]
            [Authorize]
            public async Task<IActionResult> AddReview([FromBody] ReviewDto dto)
            {
                var property = await dbContext.Set<Property>().FindAsync(dto.PropertyId);
                if (property is null)
                    return BadRequest("Invalid Property ID.");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var existingReview = await dbContext.Set<Review>()
                    .FirstOrDefaultAsync(r => r.UserId == userId && r.PropertyId == dto.PropertyId);

                if (existingReview != null)
                    return BadRequest("You have already submitted a review for this property.");

                var review = new Review
                {
                    PropertyId = dto.PropertyId,
                    UserId = userId,
                    Rating = dto.Rating,
                    Comment = dto.Comment,
                    CreatedAt = DateTime.UtcNow
                };

                await dbContext.Set<Review>().AddAsync(review);
                await dbContext.SaveChangesAsync();

                var fullReview = await dbContext.Set<Review>()
                    .Include(r => r.User)
                    .Include(r => r.Property)
                    .Where(r => r.Id == review.Id)
                    .Select(r => new ReviewDetailsDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        UserName = r.User.FirstName + " " + r.User.LastName,
                        PropertyTitle = r.Property.Title
                    })
                    .SingleOrDefaultAsync();

                return Ok(fullReview);
            }



            [HttpGet("GetByPropertyId/{propertyId}")]
            public async Task<IActionResult> GetReviewsByPropertyId(int propertyId)
            {
                var reviews = await dbContext.Set<Review>()
                    .Include(r => r.User)
                    .Include(r => r.Property)
                    .Where(r => r.PropertyId == propertyId)
                    .Select(r => new ReviewDetailsDto
                    {
                        Id = r.Id,
                        Rating = r.Rating,
                        Comment = r.Comment,
                        UserName = r.User.FirstName + " " + r.User.LastName,
                        PropertyTitle = r.Property.Title,
                        CreatedAt = r.CreatedAt
                    })
                    .ToListAsync();

                if (reviews.Count == 0)
                    return NotFound("No reviews found for this property.");

                return Ok(reviews);
            }

            [HttpDelete("{id}")]
            public async Task<IActionResult> DeleteReview(int id)
            {
                var review = await dbContext.Set<Review>().FindAsync(id);
                if (review is null)
                    return NotFound("Review not found.");

                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                if (review.UserId != userId)
                    return Forbid("You are not authorized to delete this review.");

                dbContext.Set<Review>().Remove(review);
                await dbContext.SaveChangesAsync();

                return Ok(new { Message = "Review deleted successfully.", review.Id });
            }

        }
    }
}
