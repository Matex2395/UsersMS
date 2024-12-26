using System.Security.Cryptography.X509Certificates;

namespace LoginMS.Services
{
    public interface IEmailSender
    {
        Task SendVerificationCodeAsync(string email, string code);
    }
}
