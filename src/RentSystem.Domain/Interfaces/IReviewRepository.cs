using RentSystem.Domain.Entities;

namespace RentSystem.Domain.Interfaces
{
    public interface IReviewRepository : IRepository<Review>
    {
        Task<IEnumerable<Review>> GetReviewsByPropertyAsync(int propertyId);
        Task<IEnumerable<Review>> GetReviewsByUserAsync(int userId);
    }
}
