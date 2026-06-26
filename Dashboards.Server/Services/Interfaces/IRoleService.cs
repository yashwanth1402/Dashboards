using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Models.Entities;

namespace Dashboards.Server.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<Role>> GetRolesAsync(string? search);
        Task<IEnumerable<RoleOptionDto>> GetRoleOptionsAsync();
        Task<Role?> GetRoleByIdAsync(int id);
        Task<Role> CreateRoleAsync(Role role);
        Task UpdateRoleAsync(int id, Role role);
        Task DeleteRoleAsync(int id);
    }
}
