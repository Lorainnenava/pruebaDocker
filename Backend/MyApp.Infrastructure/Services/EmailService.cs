using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MyApp.Application.DTOs.Common;
using MyApp.Application.Interfaces.Infrastructure;

namespace MyApp.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, IOptions<EmailSettings> emailSettings)
        {
            _logger = logger;
            _emailSettings = emailSettings.Value;
        }

        // Enviar un correo electrónico con cuerpo dinámico y soporte para imágenes embebidas
        public async Task<bool> SendEmail(
            string recipientAddress,
            string subject,
            string templatePath,
            Dictionary<string, string> placeholders,
            List<(string cid, string path)>? images = null)
        {
            string smtpServer = _emailSettings.SmtpServer;
            int smtpPort = _emailSettings.SmtpPort;
            string senderEmail = _emailSettings.SenderEmail;
            string senderPassword = _emailSettings.SenderPassword;

            try
            {
                // Generar el cuerpo del correo a partir de la plantilla y los marcadores
                string bodyEmail = GenerateEmailBody(templatePath, placeholders);

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Nombre de la App", senderEmail));
                message.To.Add(new MailboxAddress("", recipientAddress));
                message.Subject = subject;

                // Crear la parte HTML con imágenes embebidas
                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = bodyEmail
                };

                // Agregar las imágenes embebidas
                if (images != null)
                {
                    foreach (var (cid, path) in images)
                    {
                        if (!string.IsNullOrEmpty(path) && File.Exists(path))
                        {
                            var image = bodyBuilder.LinkedResources.Add(path);
                            image.ContentId = cid;
                        }
                    }
                }

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(smtpServer, smtpPort, MailKit.Security.SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(senderEmail, senderPassword);
                    await client.SendAsync(message);

                    await client.DisconnectAsync(true);
                }

                _logger.LogInformation("Correo enviado exitosamente a {RecipientAddress}.", recipientAddress);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar correo a {RecipientAddress}.", recipientAddress);
                throw new Exception(ex.Message);
            }
        }

        public string GenerateEmailBody(string templatePath, Dictionary<string, string> placeholders)
        {
            string template = File.ReadAllText(templatePath);

            // Reemplazar marcadores con valores dinámicos
            foreach (var placeholder in placeholders)
            {
                template = template.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
            }

            return template;
        }
    }
}
