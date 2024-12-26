using LoginMS.Models;

namespace LoginMS.Services
{
    public interface IEmailService
    {
        Task Send(EmailMetadata emailMetadata);
    }
}
