namespace MyApp.Application.Interfaces.UseCases.UserSessions
{
    public interface IUserSessionRevokedUseCase
    {
        Task<bool> Execute(string RefreshToken);
    }
}
