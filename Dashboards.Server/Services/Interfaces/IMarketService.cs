using Dashboards.Server.Models.DTOs;

namespace Dashboards.Server.Services.Interfaces
{
    public interface IMarketService
    {
        Task<List<MarketDto>> GetMarketsAsync();
    }
}