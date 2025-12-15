using Microsoft.AspNetCore.Http;

namespace RentSystem.Application.DTOs
{
    public class PropertyDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public IFormFile? MainImageUrl { get; set; }
        public int OwnerId { get; set; }
    }
}
