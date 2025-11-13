using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using MyApp.Application.DTOs.Common;
using MyApp.Infrastructure.Services;

namespace MyApp.Tests.Infrastructure.Services
{
    public class EmailServiceTests
    {
        private readonly Mock<ILogger<EmailService>> _loggerMock;
        private readonly IOptions<EmailSettings> _emailSettings;
        private readonly EmailService _emailService;

        public EmailServiceTests()
        {
            _loggerMock = new Mock<ILogger<EmailService>>();

            var settings = new EmailSettings
            {
                SmtpServer = "smtp.test.com",
                SmtpPort = 587,
                SenderEmail = "test@test.com",
                SenderPassword = "password123"
            };

            _emailSettings = Options.Create(settings);
            _emailService = new EmailService(_loggerMock.Object, _emailSettings);
        }

        [Fact]
        public void GenerateEmailBody_ShouldReplacePlaceholdersCorrectly()
        {
            string templatePath = Path.GetTempFileName();
            File.WriteAllText(templatePath, "Hola, {{Name}}. Bienvenido a {{App}}.");

            var placeholders = new Dictionary<string, string>
            {
                { "Name", "Usuario prueba" },
                { "App", "App" }
            };

            string result = _emailService.GenerateEmailBody(templatePath, placeholders);

            Assert.Contains("Hola, Usuario prueba", result);
            Assert.Contains("Bienvenido a App", result);

            File.Delete(templatePath);
        }

        [Fact(Skip = "Prueba de integración real. Ejecutar solo si SMTP está configurado correctamente.")]
        public async Task SendEmail_ShouldReturnTrue_WhenEmailSentSuccessfully()
        {
            string templatePath = Path.GetTempFileName();
            File.WriteAllText(templatePath, "Hola, {{Name}}");
            var placeholders = new Dictionary<string, string> { { "Name", "Usuario prueba" } };

            // ¡Evita ejecutar esta prueba en entornos reales! Puede intentar enviar correos.
            bool result = await _emailService.SendEmail(
                "noexiste@test.com",
                "Asunto de prueba",
                templatePath,
                placeholders
            );

            // Asegura que solo pase si el correo se envió correctamente
            Assert.True(result);

            File.Delete(templatePath);
        }

        [Fact]
        public async Task SendEmail_ShouldReturnFalse_WhenExceptionIsThrown()
        {
            var placeholders = new Dictionary<string, string> { { "Name", "Test" } };
            string basePath = AppContext.BaseDirectory;

            string templatePath = Path.Combine(basePath, @"..\..\..\..\MyApp.Shared\TemplateEmails\VerificationTemplate.cshtml");

            // Verificamos que lance excepción al intentar enviar correo con error
            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await _emailService.SendEmail(
                    "destino@test.com",
                    "Asunto",
                    templatePath,
                    placeholders
                );
            });
        }
    }
}

