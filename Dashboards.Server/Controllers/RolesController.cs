using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dashboards.Server.Data;
using Dashboards.Server.Models.Entities;
using Dashboards.Server.Models.DTOs;

namespace Dashboards.Server.Controllers
{
    [ApiController]
    [Route("api/roles")]
    public class RolesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RolesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns a simple list for role dropdown selectors.
        /// </summary>
        [HttpGet("options")]
        public async Task<ActionResult<IEnumerable<RoleOptionDto>>> GetRoleOptions()
        {
            var roles = await _context.Roles
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.Name)
                .Select(r => new RoleOptionDto { RoleId = r.RoleId, Name = r.Name })
                .ToListAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Returns roles with market info for the grid.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleListDto>>> GetRoles([FromQuery] string? search)
        {
            var query = _context.Roles
                .Where(r => !r.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(r => r.Name.Contains(search));
            }

            var roles = await query
                .OrderBy(r => r.Name)
                .Select(r => new RoleListDto
                {
                    RoleId = r.RoleId,
                    Name = r.Name,
                    Description = r.Description,
                    Markets = r.RoleMarkets.Where(rm => rm.Market != null).Select(rm => rm.Market!.Name).ToList(),
                    UserCount = r.Users.Count(u => !u.IsDeleted),
                    CreatedDate = r.CreatedDate
                })
                .ToListAsync();

            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> GetRole(int id)
        {
            var role = await _context.Roles
                .Where(r => r.RoleId == id && !r.IsDeleted)
                .Select(r => new
                {
                    r.RoleId,
                    r.Name,
                    r.Description,
                    r.UserTypeId,
                    Markets = r.RoleMarkets.Where(rm => rm.Market != null).Select(rm => rm.Market!.Name).ToList()
                })
                .FirstOrDefaultAsync();

            if (role == null)
                return NotFound();

            return Ok(role);
        }

        /// <summary>
        /// Creates a new role with market assignments and module permissions.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult> CreateRole([FromBody] CreateRoleDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 1. Find or create a corresponding UserType entry (needed for permissions FK)
            var userType = await _context.UserTypes
                .FirstOrDefaultAsync(ut => ut.Name == dto.Name);

            if (userType == null)
            {
                userType = new UserType
                {
                    Name = dto.Name,
                    IsCreatedInFoil = false
                };
                _context.UserTypes.Add(userType);
                await _context.SaveChangesAsync();
            }

            // 2. Create the role linked to the UserType
            var role = new Role
            {
                Name = dto.Name,
                Description = dto.Description,
                UserTypeId = userType.UserTypeId,
                IsDeleted = false,
                CreatedDate = DateTime.Now
            };
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();

            // 3. Assign markets
            if (dto.MarketIds.Any())
            {
                var roleMarkets = dto.MarketIds.Select(mId => new RoleMarket
                {
                    RoleId = role.RoleId,
                    MarketId = mId
                });
                _context.RoleMarkets.AddRange(roleMarkets);
            }

            // 4. Save module permissions (linked to UserType)
            if (dto.ModulePermissions.Any())
            {
                var permEntries = dto.ModulePermissions.Select(mp => new UserTypePage
                {
                    UserTypeId = userType.UserTypeId,
                    PageId = mp.PageId,
                    HasCreateAccess = mp.CanAdd,
                    HasUpdateAccess = mp.CanEdit,
                    HasDeleteAccess = mp.CanDelete,
                    HasReadAccess = mp.CanView,
                    CreatedUserName = "system",
                    CreatedOn = DateTime.Now
                });
                _context.UserTypePages.AddRange(permEntries);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRole), new { id = role.RoleId }, new { role.RoleId, role.Name });
        }

        /// <summary>
        /// Updates role name, markets, and permissions.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id, [FromBody] CreateRoleDto dto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null || role.IsDeleted)
                return NotFound();

            // Update role name & description
            role.Name = dto.Name;
            role.Description = dto.Description;
            role.ModifiedDate = DateTime.Now;

            // Ensure UserType exists for this role
            int userTypeId;
            if (role.UserTypeId.HasValue)
            {
                userTypeId = role.UserTypeId.Value;
                // Also update UserType name to stay in sync
                var userType = await _context.UserTypes.FindAsync(userTypeId);
                if (userType != null)
                {
                    userType.Name = dto.Name;
                }
            }
            else
            {
                // Create a UserType if role doesn't have one yet (legacy data)
                var existingUt = await _context.UserTypes
                    .FirstOrDefaultAsync(ut => ut.Name == dto.Name);

                if (existingUt != null)
                {
                    userTypeId = existingUt.UserTypeId;
                }
                else
                {
                    var newUt = new UserType
                    {
                        Name = dto.Name,
                        IsCreatedInFoil = false
                    };
                    _context.UserTypes.Add(newUt);
                    await _context.SaveChangesAsync();
                    userTypeId = newUt.UserTypeId;
                }
                role.UserTypeId = userTypeId;
            }

            // Update markets: remove old, add new
            var existingMarkets = await _context.RoleMarkets
                .Where(rm => rm.RoleId == id)
                .ToListAsync();
            _context.RoleMarkets.RemoveRange(existingMarkets);

            if (dto.MarketIds.Any())
            {
                var newMarkets = dto.MarketIds.Select(mId => new RoleMarket
                {
                    RoleId = id,
                    MarketId = mId
                });
                _context.RoleMarkets.AddRange(newMarkets);
            }

            // Update permissions: remove old, add new (using the correct UserTypeId)
            var existingPerms = await _context.UserTypePages
                .Where(utp => utp.UserTypeId == userTypeId)
                .ToListAsync();
            _context.UserTypePages.RemoveRange(existingPerms);

            if (dto.ModulePermissions.Any())
            {
                var newPerms = dto.ModulePermissions.Select(mp => new UserTypePage
                {
                    UserTypeId = userTypeId,
                    PageId = mp.PageId,
                    HasCreateAccess = mp.CanAdd,
                    HasUpdateAccess = mp.CanEdit,
                    HasDeleteAccess = mp.CanDelete,
                    HasReadAccess = mp.CanView,
                    CreatedUserName = "system",
                    CreatedOn = DateTime.Now
                });
                _context.UserTypePages.AddRange(newPerms);
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);

            if (role == null)
                return NotFound();

            role.IsDeleted = true;
            role.ModifiedDate = DateTime.Now;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
