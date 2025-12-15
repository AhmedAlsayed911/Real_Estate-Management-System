using RentSystem.Domain.Entities;

namespace RentSystem.Domain.Interfaces
{
    public interface IPropertyRepository : IRepository<Property>
    {
        Task<Property?> GetPropertyWithDetailsAsync(int propertyId);
        Task<IEnumerable<Property>> GetPropertiesByOwnerAsync(int ownerId);
        Task<IEnumerable<Property>> GetPropertiesByLocationAsync(string location);
    }
}
