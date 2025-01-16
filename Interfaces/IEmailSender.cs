using System.Security.Cryptography.X509Certificates;

namespace LoginMS.Interfaces
{
    public interface IEmailSender
    {
        Task SendPasswordResetCodeAsync(string email, string code);
    }
}
