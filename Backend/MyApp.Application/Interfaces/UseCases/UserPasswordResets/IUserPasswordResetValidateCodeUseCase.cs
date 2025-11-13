using MyApp.Application.DTOs.UserPasswordResets;

namespace MyApp.Application.Interfaces.UseCases.UserPasswordResets
{
    public interface IUserPasswordResetValidateCodeUseCase
    {
        Task<bool> Execute(UserPasswordResetValidateResetCodeRequest request);
    }
}
