using RentSystem.Application.DTOs;
using RentSystem.Application.Interfaces;
using RentSystem.Domain.Entities;
using RentSystem.Domain.Interfaces;

namespace RentSystem.Application.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IRoleValidator _roleValidator;

        public AuthenticationService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IPasswordHasher passwordHasher,
            IRoleValidator roleValidator)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _roleValidator = roleValidator;
        }

        public async Task<AuthenticationResult> RegisterAsync(RegisterDto dto)
        {
            // Validate role
            if (!_roleValidator.IsValidRole(dto.Role))
            {
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid role"
                };
            }

            // Check if user already exists
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            if (existingUser != null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Email is already registered"
                };
            }

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

            // Create new user
            var user = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                PasswordHash = _passwordHasher.HashPassword(dto.Password),
                Roles = new List<string> { dto.Role.ToLower() },
                ProfilePhotoUrl = profilePhotoBase64
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true
            };
        }

        public async Task<AuthenticationResult> LoginAsync(LoginDto dto)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(dto.Email);
            
            if (user == null || !_passwordHasher.VerifyPassword(dto.Password, user.PasswordHash))
            {
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid email or password"
                };
            }

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.StoreRefreshTokenAsync(user.Id, refreshToken);

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string refreshToken)
        {
            var isValid = await _tokenService.ValidateRefreshTokenAsync(refreshToken);
            
            if (!isValid)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "Invalid refresh token"
                };
            }

            var user = await _tokenService.GetUserFromRefreshTokenAsync(refreshToken);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            await _unitOfWork.RefreshTokens.RevokeTokenAsync(refreshToken);

            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            await _tokenService.StoreRefreshTokenAsync(user.Id, newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            return new AuthenticationResult
            {
                Success = true,
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };
        }

        public async Task<bool> RevokeTokenAsync(string refreshToken)
        {
            await _unitOfWork.RefreshTokens.RevokeTokenAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        private string GetDefaultProfilePhoto()
        {
            return string.Empty;
        }
    }
}
