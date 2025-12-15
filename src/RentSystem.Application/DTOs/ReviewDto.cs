using System.ComponentModel.DataAnnotations;

namespace RentSystem.Application.DTOs
{
    public class ReviewDto
    {
        [Required]
        public int PropertyId { get; set; }
        
        [Range(1, 5)]
        public double Rating { get; set; }
        
        public string? Comment { get; set; }
    }
}
