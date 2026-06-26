using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dashboards.Server.Data;

namespace Dashboards.Server.Controllers
{
    [ApiController]
    [Route("api/pages")]
    public class PagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Returns all modules/pages for the permissions grid.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetPages()
        {
            var pages = await _context.Pages
                .OrderBy(p => p.PageOrder)
                .Select(p => new
                {
                    p.PageId,
                    p.DisplayName,
                    p.PageOrder
                })
                .ToListAsync();

            return Ok(pages);
        }

        /// <summary>
        /// Returns existing permissions for a given UserType (role).
        /// Used when editing a role to pre-fill the toggles.
        /// </summary>
        [HttpGet("permissions/{userTypeId}")]
        public async Task<IActionResult> GetPermissions(int userTypeId)
        {
            var permissions = await _context.UserTypePages
                .Where(utp => utp.UserTypeId == userTypeId)
                .Select(utp => new
                {
                    utp.PageId,
                    ModuleName = utp.Page.DisplayName,
                    CanAdd = utp.HasCreateAccess,
                    CanEdit = utp.HasUpdateAccess,
                    CanDelete = utp.HasDeleteAccess,
                    CanView = utp.HasReadAccess
                })
                .ToListAsync();

            return Ok(permissions);
        }

        /// <summary>
        /// Returns existing permissions for a given Role (looks up UserTypeId from the role).
        /// </summary>
        [HttpGet("permissions/by-role/{roleId}")]
        public async Task<IActionResult> GetPermissionsByRole(int roleId)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null || !role.UserTypeId.HasValue)
                return Ok(new List<object>()); // no permissions yet

            var permissions = await _context.UserTypePages
                .Where(utp => utp.UserTypeId == role.UserTypeId.Value)
                .Select(utp => new
                {
                    utp.PageId,
                    ModuleName = utp.Page.DisplayName,
                    CanAdd = utp.HasCreateAccess,
                    CanEdit = utp.HasUpdateAccess,
                    CanDelete = utp.HasDeleteAccess,
                    CanView = utp.HasReadAccess
                })
                .ToListAsync();

            return Ok(permissions);
        }
    }
}
