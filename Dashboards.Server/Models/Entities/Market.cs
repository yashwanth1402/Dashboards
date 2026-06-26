using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboards.Server.Models.Entities
{
    /// <summary>
    /// Represents a market/region. Maps to [Type].[Market] table.
    /// </summary>
    [Table("Market", Schema = "Type")]
    public class Market
    {
        [Key]
        [Column("MarketID")]
        public int MarketId { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(100)]
        public string Code { get; set; } = null!;

        [Column("C4UID")]
        [StringLength(50)]
        public string? C4uid { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        public bool IsSyncFromSource { get; set; }

        // Navigation properties
        public virtual ICollection<RoleMarket> RoleMarkets { get; set; } = new List<RoleMarket>();
    }
}
