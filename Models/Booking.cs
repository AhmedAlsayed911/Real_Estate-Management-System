namespace RentApi.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int RenterId { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Property Property { get; set; }
        public User Renter { get; set; }
    }
}
