using Dashboards.Server.Models.DTOs;

namespace Dashboards.Server.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserListDto>> GetUsersAsync(string? search, int? roleId, string? status);
        Task<UserListDto?> GetUserByIdAsync(long id);
        Task<UserListDto> CreateUserAsync(CreateUserDto request);
        Task UpdateUserAsync(long id, UpdateUserDto request);
        Task DeleteUserAsync(long id);
    }
}
