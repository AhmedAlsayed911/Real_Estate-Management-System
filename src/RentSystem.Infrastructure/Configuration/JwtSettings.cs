namespace RentSystem.Infrastructure.Configuration
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int LifeTime { get; set; }
        public string SigningKey { get; set; } = string.Empty;
        public int AccessTokenExpiryMinutes { get; set; }
    }
}
