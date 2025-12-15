using RentSystem.Domain.Common;

namespace RentSystem.Domain.Entities
{
    public class Review : BaseEntity, IAuditableEntity
    {
        public int PropertyId { get; set; }
        public int UserId { get; set; }

        public double Rating { get; set; }
        public string? Comment { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedAt { get; set; }

        
        public Property Property { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
