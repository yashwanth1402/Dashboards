using System.ComponentModel.DataAnnotations;

namespace Dashboards.Server.Models.DTOs
{
    /// <summary>
    /// DTO returned for role list display (Roles & Access grid).
    /// </summary>
    public class RoleListDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public List<string> Markets { get; set; } = new();
        public int UserCount { get; set; }
        public DateTime? CreatedDate { get; set; }
    }

    /// <summary>
    /// DTO for creating/updating a role (from "Add Role" modal).
    /// </summary>
    public class CreateRoleDto
    {
        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public List<int> MarketIds { get; set; } = new();

        public List<ModulePermissionDto> ModulePermissions { get; set; } = new();
    }

    /// <summary>
    /// DTO for a single module permission row in the "Add Role" modal.
    /// </summary>
    public class ModulePermissionDto
    {
        public int PageId { get; set; }
        public string ModuleName { get; set; } = null!;
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanDelete { get; set; }
        public bool CanView { get; set; }
    }

    /// <summary>
    /// Simple DTO for role dropdown lists.
    /// </summary>
    public class RoleOptionDto
    {
        public int RoleId { get; set; }
        public string Name { get; set; } = null!;
    }
}
