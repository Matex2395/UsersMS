
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

        public async Task SendVerificationCodeAsync(string email, string code)
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
                Subject = "Código de Verificación",
                Body = $"Tu código de verificación es: {code}",
                IsBodyHtml = false
            };
            mailMessage.To.Add(email);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
