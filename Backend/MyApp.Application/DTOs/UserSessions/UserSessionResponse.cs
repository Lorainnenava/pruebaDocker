namespace MyApp.Application.DTOs.UserSessions
{
    public class UserSessionResponse
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
