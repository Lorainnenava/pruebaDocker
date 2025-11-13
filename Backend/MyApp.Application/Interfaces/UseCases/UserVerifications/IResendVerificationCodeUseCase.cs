using MyApp.Application.DTOs.UserVerifications;

namespace MyApp.Application.Interfaces.UseCases.UserVerifications
{
    public interface IResendVerificationCodeUseCase
    {
        Task<bool> Execute(ResendCodeRequest request);
    }
}
