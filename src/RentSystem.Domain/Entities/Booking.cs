using RentSystem.Domain.Common;

namespace RentSystem.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int PropertyId { get; set; }
        public int RenterId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        
        public Property Property { get; set; } = null!;
        public User Renter { get; set; } = null!;
    }
}
