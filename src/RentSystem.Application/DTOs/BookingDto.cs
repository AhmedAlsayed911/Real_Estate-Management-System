namespace RentSystem.Application.DTOs
{
    public class BookingDto
    {
        public int PropertyId { get; set; }
        public int RenterId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
