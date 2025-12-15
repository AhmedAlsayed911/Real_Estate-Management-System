using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentSystem.Application.DTOs;
using RentSystem.Application.Interfaces;
using System.Security.Claims;

namespace RentSystem.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PropertiesController : ControllerBase
    {
        private readonly IPropertyService _propertyService;

        public PropertiesController(IPropertyService propertyService)
        {
            _propertyService = propertyService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllProperties()
        {
            var properties = await _propertyService.GetAllPropertiesAsync();
            return Ok(properties);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertyById(int id)
        {
            var property = await _propertyService.GetPropertyByIdAsync(id);

            if (property == null)
            {
                return NotFound(new { message = "Property not found" });
            }

            return Ok(property);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetPropertiesByOwner(int ownerId)
        {
            var properties = await _propertyService.GetPropertiesByOwnerAsync(ownerId);
            return Ok(properties);
        }

        [HttpGet("location/{location}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPropertiesByLocation(string location)
        {
            var properties = await _propertyService.GetPropertiesByLocationAsync(location);
            return Ok(properties);
        }

        [HttpPost]
        [Authorize(Roles = "owner,admin")]
        public async Task<IActionResult> CreateProperty([FromForm] PropertyDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var property = await _propertyService.CreatePropertyAsync(userId, dto);

            return CreatedAtAction(nameof(GetPropertyById), new { id = property.Id }, property);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "owner,admin")]
        public async Task<IActionResult> UpdateProperty(int id, [FromForm] PropertyUpdateDto dto)
        {
            var property = await _propertyService.UpdatePropertyAsync(id, dto);

            if (property == null)
            {
                return NotFound(new { message = "Property not found" });
            }

            return Ok(property);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "owner,admin")]
        public async Task<IActionResult> DeleteProperty(int id)
        {
            var result = await _propertyService.DeletePropertyAsync(id);

            if (!result)
            {
                return NotFound(new { message = "Property not found" });
            }

            return NoContent();
        }
    }
}
