namespace RentSystem.Application.DTOs
{
    public class ReviewDetailsDto
    {
        public int Id { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PropertyTitle { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
