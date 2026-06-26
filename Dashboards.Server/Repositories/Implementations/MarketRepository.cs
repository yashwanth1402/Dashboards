using Dashboards.Server.Data;
using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dashboards.Server.Repositories.Implementations
{
    public class MarketRepository : IMarketRepository
    {
        private readonly ApplicationDbContext _context;

        public MarketRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<MarketDto>> GetMarketsAsync()
        {
            return await _context.Markets
                .OrderBy(m => m.Name)
                .Select(m => new MarketDto
                {
                    MarketId = m.MarketId,
                    Name = m.Name,
                    Code = m.Code
                })
                .ToListAsync();
        }
    }
}