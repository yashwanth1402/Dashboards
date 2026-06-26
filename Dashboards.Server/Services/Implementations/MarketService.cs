using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Repositories.Interfaces;
using Dashboards.Server.Services.Interfaces;

namespace Dashboards.Server.Services.Implementations
{
    public class MarketService : IMarketService
    {
        private readonly IMarketRepository _marketRepository;

        public MarketService(IMarketRepository marketRepository)
        {
            _marketRepository = marketRepository;
        }

        public async Task<List<MarketDto>> GetMarketsAsync()
        {
            return await _marketRepository.GetMarketsAsync();
        }
    }
}