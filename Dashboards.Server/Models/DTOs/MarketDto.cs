namespace Dashboards.Server.Models.DTOs
{
    /// <summary>
    /// DTO for market data (multi-select dropdown).
    /// </summary>
    public class MarketDto
    {
        public int MarketId { get; set; }
        public string Name { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}
