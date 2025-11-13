using MyApp.Application.DTOs.UserSessions;

namespace MyApp.Application.Interfaces.UseCases.RefreshTokens
{
    public interface IRefreshTokenUseCase
    {
        Task<UserSessionResponse> Execute(string RefreshToken);
    }
}
