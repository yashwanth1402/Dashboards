using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Models.Entities;
using Dashboards.Server.Repositories.Interfaces;
using Dashboards.Server.Services.Interfaces;

namespace Dashboards.Server.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<RoleService> _logger;

        public RoleService(IRoleRepository roleRepository, ILogger<RoleService> logger)
        {
            _roleRepository = roleRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Role>> GetRolesAsync(string? search)
        {
            var roles = await _roleRepository.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                roles = roles.Where(r => r.Name.Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            return roles;
        }

        public async Task<IEnumerable<RoleOptionDto>> GetRoleOptionsAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new RoleOptionDto { RoleId = r.RoleId, Name = r.Name });
        }

        public async Task<Role?> GetRoleByIdAsync(int id)
        {
            return await _roleRepository.GetByIdAsync(id);
        }

        public async Task<Role> CreateRoleAsync(Role role)
        {
            var created = await _roleRepository.CreateAsync(role);
            _logger.LogInformation("Role {RoleName} created", role.Name);
            return created;
        }

        public async Task UpdateRoleAsync(int id, Role role)
        {
            if (id != role.RoleId)
                throw new InvalidOperationException("Role ID mismatch.");

            await _roleRepository.UpdateAsync(role);
        }

        public async Task DeleteRoleAsync(int id)
        {
            await _roleRepository.SoftDeleteAsync(id);
            _logger.LogInformation("Role {RoleId} deleted", id);
        }
    }
}
