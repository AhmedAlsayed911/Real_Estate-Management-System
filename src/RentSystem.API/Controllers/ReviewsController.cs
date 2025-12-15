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
    public class ReviewsController : ControllerBase
    {
        private readonly IReviewService _reviewService;

        public ReviewsController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _reviewService.GetReviewByIdAsync(id);

            if (review == null)
            {
                return NotFound(new { message = "Review not found" });
            }

            return Ok(review);
        }

        [HttpGet("property/{propertyId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByProperty(int propertyId)
        {
            var reviews = await _reviewService.GetReviewsByPropertyAsync(propertyId);
            return Ok(reviews);
        }

        [HttpGet("user/{userId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetReviewsByUser(int userId)
        {
            var reviews = await _reviewService.GetReviewsByUserAsync(userId);
            return Ok(reviews);
        }

        [HttpPost]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var review = await _reviewService.CreateReviewAsync(userId, dto);

            return CreatedAtAction(nameof(GetReviewById), new { id = review.Id }, review);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var result = await _reviewService.DeleteReviewAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Review not found" });
            }

            return NoContent();
        }
    }
}
