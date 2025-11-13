using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Interfaces.UseCases.Users
{
    public interface IUserChangePasswordUseCase
    {
        Task<bool> Execute(int userId, UserChangePasswordRequest request);
    }
}
