namespace Dashboards.Server.Models.DTOs
{
    public class LoginResponse
    {
        public long UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string? EmailAddress { get; set; }
        public int? RoleId { get; set; }
        public string Token { get; set; } = null!;
        public bool RequiresPasswordReset { get; set; }
    }
}
