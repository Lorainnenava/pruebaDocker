namespace MyApp.Application.Interfaces.Services
{
    public interface ICodeGeneratorService
    {
        Task<string> GenerateUniqueCode();
        Task<string> GenerateCodeByResetPassword();
    }
}
