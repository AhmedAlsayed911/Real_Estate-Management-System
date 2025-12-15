namespace RentSystem.Application.DTOs
{
    public class PropertyDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal PricePerNight { get; set; }
        public string Location { get; set; } = string.Empty;
        public string OwnerName { get; set; } = string.Empty;
        public string MainImageUrl { get; set; } = string.Empty;
        public int BookingCount { get; set; }
        public double AverageRating { get; set; }
    }
}
