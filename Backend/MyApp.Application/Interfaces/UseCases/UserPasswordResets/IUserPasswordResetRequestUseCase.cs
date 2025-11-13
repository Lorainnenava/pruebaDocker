using MyApp.Application.DTOs.UserPasswordResets;

namespace MyApp.Application.Interfaces.UseCases.UserPasswordResets
{
    public interface IUserPasswordResetRequestUseCase
    {
        Task<bool> Execute(UserPasswordResetRequest request);
    }
}
