using RentSystem.Application.DTOs;
using RentSystem.Application.Interfaces;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;

namespace RentSystem.Application.Services
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PropertyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PropertyDetailsDto?> GetPropertyByIdAsync(int id)
        {
            var property = await _unitOfWork.Properties.GetPropertyWithDetailsAsync(id);
            return property != null ? MapToDetailsDto(property) : null;
        }

        public async Task<IEnumerable<PropertyDetailsDto>> GetAllPropertiesAsync()
        {
            var properties = await _unitOfWork.Properties.GetAllAsync();
            return properties.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<PropertyDetailsDto>> GetPropertiesByOwnerAsync(int ownerId)
        {
            var properties = await _unitOfWork.Properties.GetPropertiesByOwnerAsync(ownerId);
            return properties.Select(MapToDetailsDto);
        }

        public async Task<IEnumerable<PropertyDetailsDto>> GetPropertiesByLocationAsync(string location)
        {
            var properties = await _unitOfWork.Properties.GetPropertiesByLocationAsync(location);
            return properties.Select(MapToDetailsDto);
        }

        public async Task<PropertyDetailsDto> CreatePropertyAsync(int ownerId, PropertyDto dto)
        {
            byte[] mainImageBytes = Array.Empty<byte>();

            if (dto.MainImageUrl != null && dto.MainImageUrl.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await dto.MainImageUrl.CopyToAsync(memoryStream);
                mainImageBytes = memoryStream.ToArray();
            }

            var property = new Property
            {
                Title = dto.Title,
                Description = dto.Description,
                Location = dto.Location,
                PricePerNight = dto.PricePerNight,
                MainImageUrl = mainImageBytes,
                OwnerId = ownerId
            };

            await _unitOfWork.Properties.AddAsync(property);
            await _unitOfWork.SaveChangesAsync();

            return MapToDetailsDto(property);
        }

        public async Task<PropertyDetailsDto?> UpdatePropertyAsync(int id, PropertyUpdateDto dto)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id);
            if (property == null)
            {
                return null;
            }

            if (!string.IsNullOrEmpty(dto.Title))
                property.Title = dto.Title;
            
            if (!string.IsNullOrEmpty(dto.Description))
                property.Description = dto.Description;
            
            if (!string.IsNullOrEmpty(dto.Location))
                property.Location = dto.Location;
            
            if (dto.PricePerNight.HasValue)
                property.PricePerNight = dto.PricePerNight.Value;
            
            if (dto.MainImageUrl != null && dto.MainImageUrl.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await dto.MainImageUrl.CopyToAsync(memoryStream);
                property.MainImageUrl = memoryStream.ToArray();
            }

            _unitOfWork.Properties.Update(property);
            await _unitOfWork.SaveChangesAsync();

            return MapToDetailsDto(property);
        }

        public async Task<bool> DeletePropertyAsync(int id)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(id);
            if (property == null)
            {
                return false;
            }

            _unitOfWork.Properties.Remove(property);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        private PropertyDetailsDto MapToDetailsDto(Property property)
        {
            return new PropertyDetailsDto
            {
                Id = property.Id,
                Title = property.Title,
                Location = property.Location,
                PricePerNight = property.PricePerNight,
                MainImageUrl = property.MainImageUrl != null && property.MainImageUrl.Length > 0
                    ? Convert.ToBase64String(property.MainImageUrl)
                    : string.Empty,
                OwnerName = property.Owner != null 
                    ? $"{property.Owner.FirstName} {property.Owner.LastName}" 
                    : string.Empty,
                BookingCount = property.Bookings?.Count ?? 0,
                AverageRating = property.Reviews?.Any() == true 
                    ? property.Reviews.Average(r => r.Rating) 
                    : 0.0
            };
        }
    }
}
