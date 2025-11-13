namespace MyApp.Application.Interfaces.Infrastructure
{
    public interface IPasswordHasherService
    {
        string HashPassword(string password);
        bool VerifyPassword(string password, string storedHash);
    }
}
