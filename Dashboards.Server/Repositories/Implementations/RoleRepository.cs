using Microsoft.EntityFrameworkCore;
using Dashboards.Server.Data;
using Dashboards.Server.Models.Entities;
using Dashboards.Server.Repositories.Interfaces;

namespace Dashboards.Server.Repositories.Implementations
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetAllAsync()
        {
            return await _context.Roles
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(int id)
        {
            return await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleId == id && !r.IsDeleted);
        }

        public async Task<Role> CreateAsync(Role role)
        {
            role.CreatedDate = DateTime.Now;
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            return role;
        }

        public async Task UpdateAsync(Role role)
        {
            role.ModifiedDate = DateTime.Now;
            _context.Entry(role).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task SoftDeleteAsync(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role != null)
            {
                role.IsDeleted = true;
                role.ModifiedDate = DateTime.Now;
                await _context.SaveChangesAsync();
            }
        }
    }
}
