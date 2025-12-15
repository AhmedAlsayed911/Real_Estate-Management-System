using RentSystem.Application.DTOs;
using RentSystem.Application.Interfaces;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;

namespace RentSystem.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ReviewDetailsDto?> GetReviewByIdAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            return review != null ? MapToDetailsDto(review) : null;
        }

        public async Task<IEnumerable<ReviewDetailsDto>> GetReviewsByPropertyAsync(int propertyId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByPropertyAsync(propertyId);
            return reviews.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<ReviewDetailsDto>> GetReviewsByUserAsync(int userId)
        {
            var reviews = await _unitOfWork.Reviews.GetReviewsByUserAsync(userId);
            return reviews.Select(MapToDetailsDto);
        }

        public async Task<ReviewDetailsDto> CreateReviewAsync(int userId, ReviewDto dto)
        {
            var review = new Review
            {
                PropertyId = dto.PropertyId,
                UserId = userId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Reviews.AddAsync(review);
            await _unitOfWork.SaveChangesAsync();

            return MapToDetailsDto(review);
        }

        public async Task<bool> DeleteReviewAsync(int id)
        {
            var review = await _unitOfWork.Reviews.GetByIdAsync(id);
            if (review == null)
            {
                return false;
            }

            _unitOfWork.Reviews.Remove(review);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private ReviewDetailsDto MapToDetailsDto(Review review)
        {
            return new ReviewDetailsDto
            {
                Id = review.Id,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UserName = review.User != null 
                    ? $"{review.User.FirstName} {review.User.LastName}" 
                    : string.Empty,
                PropertyTitle = review.Property?.Title ?? string.Empty
            };
        }
    }
}
