namespace RentApi.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int UserId { get; set; }

        public double Rating { get; set; } 
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Property Property { get; set; }
        public User User { get; set; }
    }
}
