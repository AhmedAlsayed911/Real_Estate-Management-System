using System.ComponentModel.DataAnnotations;

namespace RentApi.Dtos
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
