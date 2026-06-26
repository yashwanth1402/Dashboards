using Dashboards.Server.Models.Entities;

namespace Dashboards.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<UserInformation>> GetAllAsync();
        Task<UserInformation?> GetByIdAsync(long id);
        Task<UserInformation?> GetByUsernameAsync(string username);
        Task<UserInformation?> GetByEmailAsync(string email);
        Task<UserInformation> CreateAsync(UserInformation user);
        Task UpdateAsync(UserInformation user);
        Task SoftDeleteAsync(long id);
    }
}
