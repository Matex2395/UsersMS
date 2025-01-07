
using Microsoft.Identity.Client;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;
using System.Net.Mail;

namespace LoginMS.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendPasswordResetCodeAsync(string email, string code)
        {
            var client = new SendGridClient("tu_api_key");
            var from = new EmailAddress("elmatex9@gmail.com", "Mateo");
            var subject = "Código para Restablecer tu Contraseña";
            var to = new EmailAddress(email);
            var htmlContent = $@"
                            <h2>Restablecimiento de Contraseña</h2>
                            <p>Has solicitado restablecer tu contraseña.</p>
                            <p>Tu código de verificación es: <strong>{code}</strong></p>
                            <p>Este código expirará en 5 minutos.</p>
                            <p>Si no solicitaste este cambio, ignora este correo.</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, htmlContent);
            await client.SendEmailAsync(msg);
        }
    }
}
