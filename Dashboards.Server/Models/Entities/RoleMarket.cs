using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboards.Server.Models.Entities
{
    /// <summary>
    /// Junction table linking Roles to Markets. Maps to [Users].[RoleMarkets].
    /// </summary>
    [Table("RoleMarkets", Schema = "Users")]
    public class RoleMarket
    {
        [Key]
        [Column("RoleMarketID")]
        public long RoleMarketId { get; set; }

        [Column("RoleID")]
        public int? RoleId { get; set; }

        [Column("MarketID")]
        public int? MarketId { get; set; }

        [StringLength(100)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        [StringLength(100)]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [ForeignKey("RoleId")]
        public virtual Role? Role { get; set; }

        [ForeignKey("MarketId")]
        public virtual Market? Market { get; set; }
    }
}
