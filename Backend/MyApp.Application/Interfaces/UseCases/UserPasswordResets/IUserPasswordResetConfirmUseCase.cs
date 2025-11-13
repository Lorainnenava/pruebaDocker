using MyApp.Application.DTOs.UserPasswordResets;

namespace MyApp.Application.Interfaces.UseCases.UserPasswordResets
{
    public interface IUserPasswordResetConfirmUseCase
    {
        Task<bool> Execute(UserPasswordResetConfirmRequest request);
    }
}
