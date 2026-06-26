using Dashboards.Server.Models.DTOs;

namespace Dashboards.Server.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<LoginResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
