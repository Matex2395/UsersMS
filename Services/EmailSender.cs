
using System.Net;
using System.Net.Mail;

namespace LoginMS.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = "mateo.rosero@fisagrp.com";
            var pw = "W8lS#JKMjN7ifd&1";

            var client = new SmtpClient("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail,
                                to: email,
                                subject,
                                message
                                ));
        }
    }
}
