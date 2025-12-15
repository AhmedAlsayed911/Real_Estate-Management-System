namespace RentSystem.Application.DTOs
{
    public class BookingDetailsDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string PropertyTitle { get; set; } = string.Empty;
        public string PropertyLocation { get; set; } = string.Empty;
        public string RenterFullName { get; set; } = string.Empty;
    }
}
