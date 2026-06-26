using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Models.Entities;
using Dashboards.Server.Repositories.Interfaces;
using Dashboards.Server.Services.Interfaces;

namespace Dashboards.Server.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepository, ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<UserListDto>> GetUsersAsync(string? search, int? roleId, string? status)
        {
            var users = await _userRepository.GetAllAsync();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                users = users.Where(u =>
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.UserName.ToLower().Contains(term) ||
                    (u.EmailAddress != null && u.EmailAddress.ToLower().Contains(term)));
            }

            if (roleId.HasValue)
                users = users.Where(u => u.RoleId == roleId.Value);

            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
            {
                bool isActive = status.ToLower() == "active";
                users = users.Where(u => u.IsDeleted != isActive);
            }

            return users.Select(MapToDto);
        }

        public async Task<UserListDto?> GetUserByIdAsync(long id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user != null ? MapToDto(user) : null;
        }

        public async Task<UserListDto> CreateUserAsync(CreateUserDto request)
        {
            // Check duplicate email
            var existing = await _userRepository.GetByEmailAsync(request.EmailAddress);
            if (existing != null)
                throw new InvalidOperationException("A user with this email already exists.");

            var user = new UserInformation
            {
                UserName = request.EmailAddress,
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.EmailAddress,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Password = "MIGRATED",
                RoleId = request.RoleId,
                IsDeleted = !request.IsActive,
                UserTypeId = 1,
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };

            var created = await _userRepository.CreateAsync(user);
            _logger.LogInformation("User {Username} created successfully", user.UserName);

            return MapToDto(created);
        }

        public async Task UpdateUserAsync(long id, UpdateUserDto request)
        {
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new KeyNotFoundException("User not found.");

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.EmailAddress = request.EmailAddress;
            user.RoleId = request.RoleId;
            user.IsDeleted = !request.IsActive;
            user.ModifiedBy = "Admin";
            user.ModifiedDate = DateTime.Now;

            await _userRepository.UpdateAsync(user);
        }

        public async Task DeleteUserAsync(long id)
        {
            await _userRepository.SoftDeleteAsync(id);
        }

        private static UserListDto MapToDto(UserInformation user) => new()
        {
            UserId = user.UserId,
            UserName = user.UserName,
            FirstName = user.FirstName,
            LastName = user.LastName,
            EmailAddress = user.EmailAddress,
            RoleName = user.Role?.Name,
            RoleId = user.RoleId,
            CreatedBy = user.CreatedBy,
            CreatedDate = user.CreatedDate,
            IsActive = !user.IsDeleted
        };
    }
}
