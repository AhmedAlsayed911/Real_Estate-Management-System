namespace RentApi.Dtos
{
    public class PropertyDtoUpdate
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
        public decimal? PricePerNight { get; set; }
        public IFormFile? MainImageUrl { get; set; }

    }
}
