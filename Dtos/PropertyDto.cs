using RentApi.Models;
using System.ComponentModel.DataAnnotations;

namespace RentApi.Dtos
{
    public class PropertyDto
    {
        public string Title { get; set; }       
        public string Description { get; set; }
        public string Location { get; set; }
        public decimal PricePerNight { get; set; }
        public IFormFile? MainImageUrl { get; set; }

        public int OwnerId { get; set; }
    }
}
