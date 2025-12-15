using RentSystem.Application.DTOs;

namespace RentSystem.Application.Interfaces
{
    public interface IReviewService
    {
        Task<ReviewDetailsDto?> GetReviewByIdAsync(int id);
        Task<IEnumerable<ReviewDetailsDto>> GetReviewsByPropertyAsync(int propertyId);
        Task<IEnumerable<ReviewDetailsDto>> GetReviewsByUserAsync(int userId);
        Task<ReviewDetailsDto> CreateReviewAsync(int userId, ReviewDto dto);
        Task<bool> DeleteReviewAsync(int id);
    }
}
