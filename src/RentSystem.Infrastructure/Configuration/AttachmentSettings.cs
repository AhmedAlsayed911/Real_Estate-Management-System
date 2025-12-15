namespace RentSystem.Infrastructure.Configuration
{
    public class AttachmentSettings
    {
        public string AllowedExtensions { get; set; } = string.Empty;
        public int MaxByteLength { get; set; }
    }
}
