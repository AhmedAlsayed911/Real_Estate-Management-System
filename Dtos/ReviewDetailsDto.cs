namespace RentApi.Dtos
{
    public class ReviewDetailsDto
    {
        public int Id { get; set; }
        public double Rating { get; set; }
        public string? Comment { get; set; }

        public string UserName { get; set; }
        public string PropertyTitle { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
