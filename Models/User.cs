using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace RentApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [MaxLength(20)]
        public string FirstName { get; set; }
        [MaxLength(20)]
        public string LastName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public List<string> Role { get; set; } 
        public string Password {  get; set; }
        public string? ProfilePhotoUrl { get; set; }
        public ICollection<Property> Properties { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }
    }
}
