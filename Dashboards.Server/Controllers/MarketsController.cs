using Dashboards.Server.Models.DTOs;
using Dashboards.Server.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Dashboards.Server.Controllers
{
    [ApiController]
    [Route("api/markets")]
    public class MarketsController : ControllerBase
    {
        private readonly IMarketService _marketService;

        public MarketsController(IMarketService marketService)
        {
            _marketService = marketService;
        }

        [HttpGet]
        public async Task<ActionResult<List<MarketDto>>> GetMarkets()
        {
            var markets = await _marketService.GetMarketsAsync();
            return Ok(markets);
        }
    }
}