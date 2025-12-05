using Microsoft.AspNetCore.Mvc.ViewEngines;
using System.ComponentModel.DataAnnotations;

namespace RentApi.Models
{
    public class Property
    {
        public int Id { get; set; }
        public string Title { get; set; }
        [MaxLength(100)]
        public string Description { get; set; }
        [MaxLength(100)]
        public string Location { get; set; }
        public decimal PricePerNight { get; set; }
        public byte[] MainImageUrl { get; set; } 

        public int OwnerId { get; set; }
        public User Owner { get; set; }

        public ICollection<Booking> Bookings { get; set; }
        public ICollection<Review> Reviews { get; set; }
    }
}
