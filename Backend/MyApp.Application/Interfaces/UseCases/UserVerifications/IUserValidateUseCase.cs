using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Interfaces.UseCases.UserVerifications
{
    public interface IUserValidateUseCase
    {
        Task<bool> Execute(UserCodeValidationRequest request);
    }
}
