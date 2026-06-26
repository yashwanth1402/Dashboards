using Dashboards.Server.Models.Entities;

namespace Dashboards.Server.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(int id);
        Task<Role> CreateAsync(Role role);
        Task UpdateAsync(Role role);
        Task SoftDeleteAsync(int id);
    }
}
