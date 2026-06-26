using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dashboards.Server.Models.Entities
{
    /// <summary>
    /// Permissions matrix: which UserType has Add/Edit/Delete/View access to which Page (module).
    /// Maps to [Security].[UserTypePages].
    /// </summary>
    [Table("UserTypePages", Schema = "Security")]
    [PrimaryKey("UserTypeId", "PageId")]
    public class UserTypePage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserTypePageId { get; set; }

        public int PageId { get; set; }

        public int UserTypeId { get; set; }

        public bool HasCreateAccess { get; set; }

        public bool HasReadAccess { get; set; }

        public bool HasUpdateAccess { get; set; }

        public bool HasDeleteAccess { get; set; }

        [Required]
        [StringLength(50)]
        public string CreatedUserName { get; set; } = null!;

        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        // Navigation properties
        [ForeignKey("PageId")]
        public virtual Page Page { get; set; } = null!;

        [ForeignKey("UserTypeId")]
        public virtual UserType UserType { get; set; } = null!;
    }
}
