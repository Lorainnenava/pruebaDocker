namespace MyApp.Application.Interfaces.Infrastructure
{
    public interface IEmailService
    {
        Task<bool> SendEmail(string recipientAddress, string subject, string templatePath, Dictionary<string, string> placeholders, List<(string cid, string path)>? images = null);
        string GenerateEmailBody(string templatePath, Dictionary<string, string> placeholders);
    }
}
