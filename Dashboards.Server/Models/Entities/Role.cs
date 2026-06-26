using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboards.Server.Models.Entities
{
    [Table("Roles", Schema = "Type")]
    public class Role
    {
        [Key]
        [Column("RoleID")]
        public int RoleId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [StringLength(500)]
        public string? Description { get; set; }

        [Column("UserTypeID")]
        public int? UserTypeId { get; set; }

        public bool IsDeleted { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public virtual ICollection<UserInformation> Users { get; set; } = new List<UserInformation>();
        public virtual ICollection<RoleMarket> RoleMarkets { get; set; } = new List<RoleMarket>();

        [ForeignKey("UserTypeId")]
        public virtual UserType? UserType { get; set; }
    }
}
