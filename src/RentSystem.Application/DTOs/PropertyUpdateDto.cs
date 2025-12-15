using Microsoft.AspNetCore.Http;

namespace RentSystem.Application.DTOs
{
    public class PropertyUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? PricePerNight { get; set; }
        public IFormFile? MainImageUrl { get; set; }
    }
}
