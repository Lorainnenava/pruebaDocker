using MyApp.Application.DTOs.Users;

namespace MyApp.Application.Interfaces.UseCases.Users
{
    public interface IUserGetByIdUseCase
    {
        public Task<UserResponse> Execute(int UserId);
    }
}
