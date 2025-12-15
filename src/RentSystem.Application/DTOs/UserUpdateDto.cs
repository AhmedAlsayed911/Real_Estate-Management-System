using Microsoft.AspNetCore.Http;

namespace RentSystem.Application.DTOs
{
    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public IFormFile? ProfilePhoto { get; set; }
    }
}
