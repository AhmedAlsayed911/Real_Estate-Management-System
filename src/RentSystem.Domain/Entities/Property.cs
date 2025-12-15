using RentSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RentSystem.Domain.Entities
{
    public class Property : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        
        [MaxLength(100)]
        public string Location { get; set; } = string.Empty;
        
        public decimal PricePerNight { get; set; }
        
        public byte[] MainImageUrl { get; set; } = Array.Empty<byte>();

        public int OwnerId { get; set; }
        
        // Navigation properties
        public User Owner { get; set; } = null!;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
