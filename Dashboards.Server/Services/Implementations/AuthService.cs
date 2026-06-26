using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Models.Entities;
using Dashboards.Server.Repositories.Interfaces;
using Dashboards.Server.Services.Interfaces;

namespace Dashboards.Server.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IConfiguration configuration,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            _logger.LogInformation("Login attempt for user {Username}", request.Username);

            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
            {
                _logger.LogWarning("Invalid login attempt for user {Username}", request.Username);
                throw new UnauthorizedAccessException("Invalid username or password.");
            }

            // HYBRID AUTH: Check if user has migrated to BCrypt
            if (user.HasMigratedPassword)
            {
                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Invalid password for user {Username}", request.Username);
                    throw new UnauthorizedAccessException("Invalid username or password.");
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("User {Username} logged in successfully", request.Username);

                return new LoginResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    RoleId = user.RoleId,
                    Token = token,
                    RequiresPasswordReset = false
                };
            }
            else
            {
                // Old user without BCrypt hash - needs password reset
                _logger.LogInformation("User {Username} requires password migration", request.Username);

                return new LoginResponse
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    EmailAddress = user.EmailAddress,
                    RoleId = user.RoleId,
                    Token = string.Empty,
                    RequiresPasswordReset = true
                };
            }
        }

        public async Task<LoginResponse> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);

            if (user == null)
                throw new KeyNotFoundException("User not found.");

            // Hash the new password with BCrypt
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
            user.ModifiedBy = $"{user.FirstName} {user.LastName}";
            user.ModifiedDate = DateTime.Now;

            await _userRepository.UpdateAsync(user);

            var token = GenerateJwtToken(user);
            _logger.LogInformation("User {Username} reset password successfully", request.Username);

            return new LoginResponse
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.EmailAddress,
                RoleId = user.RoleId,
                Token = token,
                RequiresPasswordReset = false
            };
        }

        private string GenerateJwtToken(UserInformation user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName),
                new Claim(ClaimTypes.Role, user.RoleId?.ToString() ?? "0")
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    double.Parse(jwtSettings["ExpirationHours"] ?? "8")),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
