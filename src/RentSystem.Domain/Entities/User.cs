using RentSystem.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace RentSystem.Domain.Entities
{
    public class User : BaseEntity
    {
        [MaxLength(20)]
        public string FirstName { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string LastName { get; set; } = string.Empty;
        
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        public List<string> Roles { get; set; } = new();
        
        public string PasswordHash { get; set; } = string.Empty;
        
        public string? ProfilePhotoUrl { get; set; }
        
        
        public ICollection<Property> Properties { get; set; } = new List<Property>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
