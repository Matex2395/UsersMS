using System.Security.Cryptography.X509Certificates;

namespace LoginMS.Services
{
    public interface IEmailSender
    {
        Task SendPasswordResetCodeAsync(string email, string code);
    }
}
