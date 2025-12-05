namespace RentApi.Dtos
{
    public class PropertyDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public decimal PricePerNight { get; set; }
        public string Location { get; set; }
        public string OwnerName { get; set; }
        public string MainImageUrl { get; set; }
        public int BookingCount { get; set; }
        public double AverageRating { get; set; }
    }
}
