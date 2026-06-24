using System.Net.Mime;
using Microsoft.AspNetCore.Mvc;

namespace Dashboards.Server.Controllers
{
    [ApiController]
    [Route("api/employees")]
    [Produces(MediaTypeNames.Application.Json)]
    public class EmployeesController : ControllerBase
    {
        private const string EmployeesApiUrl = "https://dummyjson.com/users";
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeesController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("EmployeesApi");
                using var response = await httpClient.GetAsync(EmployeesApiUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode(
                        StatusCodes.Status502BadGateway,
                        new
                        {
                            message = "Employee data provider is unavailable.",
                            statusCode = (int)response.StatusCode
                        });
                }

                var data = await response.Content.ReadFromJsonAsync<object>();

                return Ok(data);
            }
            catch (TaskCanceledException)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        message = "Employee data provider request timed out."
                    });
            }
            catch (HttpRequestException)
            {
                return StatusCode(
                    StatusCodes.Status502BadGateway,
                    new
                    {
                        message = "Employee data provider could not be reached."
                    });
            }
        }
    }
}
