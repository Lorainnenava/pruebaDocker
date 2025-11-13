namespace MyApp.Application.Interfaces.UseCases.Users
{
    public interface IUserSetActiveStatusUseCase
    {
        Task<bool> Execute(int UserId);
    }
}
