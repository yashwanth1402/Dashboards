using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Dashboards.Server.Models.Entities
{
    /// <summary>
    /// Represents a user type/category (e.g., Admin, Standard User).
    /// Maps to [Type].[Users] table.
    /// </summary>
    [Table("Users", Schema = "Type")]
    [Index("Name", IsUnique = true)]
    public class UserType
    {
        [Key]
        [Column("UserTypeID")]
        public int UserTypeId { get; set; }

        [Required]
        [StringLength(250)]
        public string Name { get; set; } = null!;

        [Column("IsCreatedInFOIL")]
        public bool IsCreatedInFoil { get; set; }

        // Navigation properties
        public virtual ICollection<UserInformation> UsersInformations { get; set; } = new List<UserInformation>();
        public virtual ICollection<UserTypePage> UserTypePages { get; set; } = new List<UserTypePage>();
    }
}
