using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboards.Server.Models.Entities
{
    /// <summary>
    /// Represents a module/page in the system. Maps to [Security].[Pages].
    /// These are the modules shown in the "Module Permissions" grid (e.g. Main Dashboard, Insurance Policies, etc.)
    /// </summary>
    [Table("Pages", Schema = "Security")]
    public class Page
    {
        [Key]
        public int PageId { get; set; }

        [Required]
        [StringLength(250)]
        public string DisplayName { get; set; } = null!;

        [Required]
        [StringLength(400)]
        public string SourcePageName { get; set; } = null!;

        public int PageOrder { get; set; }

        // Navigation properties
        public virtual ICollection<UserTypePage> UserTypePages { get; set; } = new List<UserTypePage>();
    }
}
