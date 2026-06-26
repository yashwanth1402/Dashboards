using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dashboards.Server.Data;
using Dashboards.Server.Models.Entities;
using Dashboards.Server.Models.DTOs;

namespace Dashboards.Server.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserListDto>>> GetUsers(
            [FromQuery] string? search,
            [FromQuery] int? roleId,
            [FromQuery] string? status)
        {
            var query = _context.Users
                .Include(u => u.Role)
                .AsQueryable();

            // Filter by search (name, email, or username)
            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.ToLower();
                query = query.Where(u =>
                    u.FirstName.ToLower().Contains(term) ||
                    u.LastName.ToLower().Contains(term) ||
                    u.UserName.ToLower().Contains(term) ||
                    (u.EmailAddress != null && u.EmailAddress.ToLower().Contains(term)));
            }

            // Filter by role
            if (roleId.HasValue)
            {
                query = query.Where(u => u.RoleId == roleId.Value);
            }

            // Filter by status
            if (!string.IsNullOrWhiteSpace(status) && status.ToLower() != "all")
            {
                bool isActive = status.ToLower() == "active";
                query = query.Where(u => u.IsDeleted != isActive);
            }

            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Select(u => new UserListDto
                {
                    UserId = u.UserId,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    EmailAddress = u.EmailAddress,
                    RoleName = u.Role != null ? u.Role.Name : null,
                    RoleId = u.RoleId,
                    CreatedBy = u.CreatedBy,
                    CreatedDate = u.CreatedDate,
                    IsActive = !u.IsDeleted
                })
                .ToListAsync();

            return Ok(users);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserListDto>> GetUser(long id)
        {
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.UserId == id);

            if (user == null)
                return NotFound();

            return Ok(new UserListDto
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
            });
        }

        [HttpPost]
        public async Task<ActionResult<UserListDto>> CreateUser([FromBody] CreateUserDto request)
        {
            // Check if email already exists
            var existing = await _context.Users
                .FirstOrDefaultAsync(u => u.EmailAddress == request.EmailAddress && !u.IsDeleted);

            if (existing != null)
                return BadRequest(new { message = "A user with this email already exists." });

            var user = new UserInformation
            {
                UserName = $"{request.FirstName}.{request.LastName}".ToLower(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                EmailAddress = request.EmailAddress,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Password = "MIGRATED", // Legacy field - not used for new users
                RoleId = request.RoleId,
                IsDeleted = !request.IsActive,
                UserTypeId = 1, // Default user type
                CreatedBy = "Admin",
                CreatedDate = DateTime.Now
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Reload with role
            await _context.Entry(user).Reference(u => u.Role).LoadAsync();

            return CreatedAtAction(nameof(GetUser), new { id = user.UserId }, new UserListDto
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
            });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, [FromBody] UpdateUserDto request)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.FirstName = request.FirstName;
            user.LastName = request.LastName;
            user.EmailAddress = request.EmailAddress;
            user.RoleId = request.RoleId;
            user.IsDeleted = !request.IsActive;
            user.ModifiedBy = "Admin";
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
                return NotFound();

            user.IsDeleted = true;
            user.ModifiedBy = "Admin";
            user.ModifiedDate = DateTime.Now;

            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
