using RentSystem.Domain.Common;

namespace RentSystem.Domain.Entities
{
    public class PasswordResetCode : BaseEntity
    {
        public string Code { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
