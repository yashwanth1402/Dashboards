using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboards.Server.Models.Entities
{
    [Table("Information", Schema = "Users")]
    public class UserInformation
    {
        [Key]
        [Column("UserID")]
        public long UserId { get; set; }

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string Password { get; set; } = null!;

        [StringLength(200)]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = null!;

        [Column("RoleID")]
        public int? RoleId { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        [Column("UserTypeID")]
        public int UserTypeId { get; set; }

        [StringLength(50)]
        public string? EmailAddress { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("UserTypeId")]
        public virtual UserType? UserType { get; set; }

        // Computed properties
        [NotMapped]
        public bool HasMigratedPassword => !string.IsNullOrEmpty(PasswordHash);

        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        [NotMapped]
        public bool IsActive => !IsDeleted;
    }
}
