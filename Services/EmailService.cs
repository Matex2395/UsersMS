using FluentEmail.Core;
using LoginMS.Models;

namespace LoginMS.Services
{
    public class EmailService : IEmailService
    {
        private readonly IFluentEmail _fluentEmail;

        public EmailService(IFluentEmail fluentEmail)
        {
            _fluentEmail = fluentEmail;
        }

        public async Task Send(EmailMetadata emailMetadata)
        {
            await _fluentEmail.To(emailMetadata.vls_toaddress)
                .Subject(emailMetadata.vls_subject)
                .Body(emailMetadata.vls_body)
                .SendAsync();
        }
    }
}
