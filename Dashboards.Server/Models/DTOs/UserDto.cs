using System.ComponentModel.DataAnnotations;

namespace Dashboards.Server.Models.DTOs
{
    /// <summary>
    /// DTO returned for user list display (Users tab grid).
    /// </summary>
    public class UserListDto
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string FullName => $"{FirstName} {LastName}";
        public string? EmailAddress { get; set; }
        public string? RoleName { get; set; }
        public int? RoleId { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for creating a new user (from "Create New User" modal).
    /// </summary>
    public class CreateUserDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        [EmailAddress]
        public string EmailAddress { get; set; } = null!;

        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        public string Password { get; set; } = null!;

        [Required]
        public int RoleId { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// DTO for updating an existing user.
    /// </summary>
    public class UpdateUserDto
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [StringLength(50)]
        [EmailAddress]
        public string? EmailAddress { get; set; }

        public int? RoleId { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
