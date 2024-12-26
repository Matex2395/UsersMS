using System.Security.Cryptography.X509Certificates;

namespace LoginMS.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
