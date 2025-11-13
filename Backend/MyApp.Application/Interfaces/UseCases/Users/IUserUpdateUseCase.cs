using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Interfaces.UseCases.Users
{
    public interface IUserUpdateUseCase
    {
        Task<UserResponse> Execute(int UserId, UserUpdateRequest request);
    }
}
