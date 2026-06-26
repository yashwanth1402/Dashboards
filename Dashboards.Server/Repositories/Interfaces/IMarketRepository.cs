using Dashboards.Server.Models.DTOs;

namespace Dashboards.Server.Repositories.Interfaces
{
    public interface IMarketRepository
    {
        Task<List<MarketDto>> GetMarketsAsync();
    }
}