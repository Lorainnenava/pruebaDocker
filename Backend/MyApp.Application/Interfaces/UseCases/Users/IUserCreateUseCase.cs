using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Interfaces.UseCases.Users
{
    public interface IUserCreateUseCase
    {
        Task<UserResponse> Execute(UserCreateRequest request);
    }
}
