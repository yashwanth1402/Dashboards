using Microsoft.AspNetCore.Mvc;

namespace Dashboards.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly HttpClient _httpClient;

        public EmployeesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            var response = await _httpClient
                .GetAsync("https://dummyjson.com/users");

            var data = await response.Content
                .ReadFromJsonAsync<object>();

            return Ok(data);
        }
    }
}