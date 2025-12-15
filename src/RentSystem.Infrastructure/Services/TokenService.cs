using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RentSystem.Application.Interfaces;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RentSystem.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _signingKey;
        private readonly int _accessTokenExpiryMinutes;
        private readonly IUnitOfWork _unitOfWork;

        public TokenService(
            IConfiguration configuration,
            IUnitOfWork unitOfWork)
        {
            _issuer = configuration["JWT:Issuer"] ?? throw new ArgumentNullException("JWT:Issuer");
            _audience = configuration["JWT:Audience"] ?? throw new ArgumentNullException("JWT:Audience");
            _signingKey = configuration["JWT:SigningKey"] ?? throw new ArgumentNullException("JWT:SigningKey");
            _accessTokenExpiryMinutes = int.Parse(configuration["JWT:AccessTokenExpiryMinutes"] ?? "60");
            _unitOfWork = unitOfWork;
        }

        public string GenerateAccessToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
            };

            foreach (var role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_signingKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_accessTokenExpiryMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        public async Task<string> StoreRefreshTokenAsync(int userId, string token)
        {
            var refreshToken = new RefreshToken
            {
                Token = token,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            
            return token;
        }

        public async Task<bool> ValidateRefreshTokenAsync(string token)
        {
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            
            if (refreshToken == null)
                return false;
            
            if (refreshToken.IsRevoked)
                return false;
            
            if (refreshToken.ExpiresAt < DateTime.UtcNow)
                return false;
            
            return true;
        }

        public async Task<User?> GetUserFromRefreshTokenAsync(string token)
        {
            var refreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(token);
            return refreshToken?.User;
        }
    }
}
