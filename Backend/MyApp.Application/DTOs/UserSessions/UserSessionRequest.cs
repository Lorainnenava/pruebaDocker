namespace MyApp.Application.DTOs.UserSessions
{
    public class UserSessionRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }
}
