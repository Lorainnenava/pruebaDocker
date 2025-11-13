using MyApp.Application.DTOs.UserSessions;

namespace MyApp.Application.Interfaces.UseCases.UserSessions
{
    public interface IUserSessionsCreateUseCase
    {
        Task<UserSessionResponse> Execute(UserSessionRequest request);
    }
}
