
using Microsoft.Identity.Client;
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
            using var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:Port"]!),
                Credentials = new NetworkCredential(_configuration["Email:Username"],
                                                    _configuration["Email:Password"]),
                EnableSsl = true,
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Email:Username"]!),
                Subject = "Código para Restablecer tu Contraseña",
                Body = $@"
                        <h2>Restablecimiento de Contraseña</h2>
                        <p>Has solicitado restablecer tu contraseña.</p>
                        <p>Tu código de verificación es: <strong>{code}</strong></p>
                        <p>Este código expirará en 5 minutos.</p>
                        <p>Si no solicitaste este cambio, ignora este correo.</p>",
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
