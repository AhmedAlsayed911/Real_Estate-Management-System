using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentApi.Dtos;
using RentApi.Helpers;
using RentApi.Models;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;

namespace RentApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ApplicationDbContext dbContext , Roles role , TokenGenerator tokenGenerator) : ControllerBase
    {
        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> RegisterUser([FromForm] RegisterDto dto)
        {
            if (!role.AllowedRoles.Contains(dto.Role.ToLower()))
                return BadRequest("Invalid Role");

            var iseExist = await dbContext.Set<User>().AnyAsync(e => e.Email == dto.Email);
            if (iseExist)
                return BadRequest("Email is Already Registred!");

            string? profilePhotoBase64 = null;

            if (dto.ProfilePhoto != null && dto.ProfilePhoto.Length > 0)
            {
                using var memoryStream = new MemoryStream();
                await dto.ProfilePhoto.CopyToAsync(memoryStream);
                profilePhotoBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }
            else
            {
                
                profilePhotoBase64 = GetDefaultProfilePhoto();
            }


            var user = new User
            {
                FirstName = dto.FirstName,
                Email = dto.Email,
                LastName = dto.LastName,
                Password = dto.Password,
                Role = new List<string> { dto.Role.ToLower() },
                ProfilePhotoUrl = profilePhotoBase64  
            };

            await dbContext.Set<User>().AddAsync(user);
            await dbContext.SaveChangesAsync();

            return Ok("User registered successfully");
        }
        //[HttpPost]
        //[Route("Login")]
        //public async Task<IActionResult> Login([FromBody] LoginDto dto)
        //{
        //    var user = await dbContext.Set<User>()
        //        .SingleOrDefaultAsync(x => x.Email == dto.Email && x.Password == dto.Password);

        //    if (user is null)
        //        return Unauthorized("Invalid Email or Password!");

        //    var accessToken = tokenGenerator.GenerateToken(user);

        //    var refreshToken = new RefreshToken
        //    {
        //        Token = Guid.NewGuid().ToString(),
        //        CreatedAt = DateTime.UtcNow,
        //        ExpiresAt = DateTime.UtcNow.AddDays(7),
        //        IsRevoked = false,
        //        UserId = user.Id
        //    };

        //    await dbContext.Set<RefreshToken>().AddAsync(refreshToken);
        //    await dbContext.SaveChangesAsync();

        //    Response.Cookies.Append("RefreshToken", refreshToken.Token, new CookieOptions
        //    {
        //        HttpOnly = true,
        //        Secure = true, 
        //        SameSite = SameSiteMode.Strict,
        //        Expires = refreshToken.ExpiresAt
        //    });

        //    return Ok(new
        //    {
        //        AccessToken = accessToken
        //    });
        //}

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await dbContext.Set<User>()
                .SingleOrDefaultAsync(x => x.Email == dto.Email && x.Password == dto.Password);

            if (user is null)
                return Unauthorized("Invalid Email or Password!");

            var accessToken = tokenGenerator.GenerateToken(user);
            var refreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                UserId = user.Id
            };

            await dbContext.Set<RefreshToken>().AddAsync(refreshToken);
            await dbContext.SaveChangesAsync();

            
            Response.Cookies.Append("RefreshToken", refreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false, 
                SameSite = SameSiteMode.None, 
                Expires = refreshToken.ExpiresAt,
                Domain = null 
            });

            return Ok(new
            {
                AccessToken = accessToken,
                User = new
                {
                    Id = user.Id,
                    Email = user.Email,
                    
                }
            });
        }

        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var oldRefreshToken = Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(oldRefreshToken))
                return Unauthorized("No refresh token found.");

            var storedToken = await dbContext.Set<RefreshToken>()
                .Include(r => r.User)
                .FirstOrDefaultAsync(r => r.Token == oldRefreshToken);

            if (storedToken is null || storedToken.IsRevoked || storedToken.ExpiresAt < DateTime.UtcNow)
                return Unauthorized("Invalid or expired refresh token.");

            storedToken.IsRevoked = true;

            var newRefreshToken = new RefreshToken
            {
                Token = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false,
                UserId = storedToken.UserId
            };

            await dbContext.Set<RefreshToken>().AddAsync(newRefreshToken);
            await dbContext.SaveChangesAsync();

            
            Response.Cookies.Append("RefreshToken", newRefreshToken.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                SameSite = SameSiteMode.None, 
                Expires = newRefreshToken.ExpiresAt,
                Domain = null 
            });

            var accessToken = tokenGenerator.GenerateToken(storedToken.User);

            return Ok(new
            {
                AccessToken = accessToken
            });
        }
        [Authorize]
        [HttpPost("Revoke")]
        public async Task<IActionResult> RevokeRefreshToken()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var refreshToken = Request.Cookies["RefreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("No refresh token provided.");

            var storedToken = await dbContext.Set<RefreshToken>()
                .FirstOrDefaultAsync(t => t.Token == refreshToken && t.UserId == userId);

            if (storedToken is null)
                return NotFound("Refresh token not found or doesn't belong to you.");

            if (storedToken.IsRevoked)
                return BadRequest("Token is already revoked.");

            storedToken.IsRevoked = true;
            await dbContext.SaveChangesAsync();

            Response.Cookies.Delete("RefreshToken");

            return Ok("Refresh token revoked successfully.");
        }

        [Authorize]
        [HttpPut("Update")]
        public async Task<IActionResult> UpdateUser([FromForm] UpdateUserDto dto, IFormFile? profilePhoto)
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await dbContext.Set<User>().FindAsync(userId);
            if (user is null)
                return NotFound("User not found.");

            if (!string.IsNullOrEmpty(dto.FirstName))
                user.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName))
                user.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Password))
                user.Password = dto.Password;

            
            if (profilePhoto != null)
            {
                using var memoryStream = new MemoryStream();
                await profilePhoto.CopyToAsync(memoryStream);
                user.ProfilePhotoUrl = Convert.ToBase64String(memoryStream.ToArray());
            }

            await dbContext.SaveChangesAsync();
            return Ok("User updated successfully.");
        }

        [Authorize]
        [HttpDelete("Delete")]
        public async Task<IActionResult> DeleteUser()
        {
            var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var user = await dbContext.Set<User>().FindAsync(userId);

            if (user is null)
                return NotFound("User not found.");

            dbContext.Set<User>().Remove(user);
            await dbContext.SaveChangesAsync();

            return Ok("User deleted successfully.");
        }

        [Authorize]
        [HttpGet("Me")]
        public IActionResult GetCurrentUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            var role = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            var userName = dbContext.Set<User>().Find(int.Parse(userId));
            var fullName = userName.FirstName +" "+userName.LastName;
            return Ok(new { userId, fullName, email, role, profilePhoto = userName.ProfilePhotoUrl });
        }
        private string GetDefaultProfilePhoto()
        {
            return null;
        }
    }

}
