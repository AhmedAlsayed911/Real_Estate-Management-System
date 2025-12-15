using RentSystem.Application.DTOs;

namespace RentSystem.Application.Interfaces
{
    public interface IPropertyService
    {
        Task<PropertyDetailsDto?> GetPropertyByIdAsync(int id);
        Task<IEnumerable<PropertyDetailsDto>> GetAllPropertiesAsync();
        Task<IEnumerable<PropertyDetailsDto>> GetPropertiesByOwnerAsync(int ownerId);
        Task<IEnumerable<PropertyDetailsDto>> GetPropertiesByLocationAsync(string location);
        Task<PropertyDetailsDto> CreatePropertyAsync(int ownerId, PropertyDto dto);
        Task<PropertyDetailsDto?> UpdatePropertyAsync(int id, PropertyUpdateDto dto);
        Task<bool> DeletePropertyAsync(int id);
    }
}
