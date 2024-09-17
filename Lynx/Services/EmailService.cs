using Lynx.IServices;
using System.Net;
using System.Net.Mail;

namespace Lynx.Services
{
    public class OneService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public OneService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = _configuration["EmailSettings:Sender"];
            var pw = _configuration["EmailSettings:Password"];
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail!, to: email, subject, message)
            );
        }
    }

    public class TwoService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public TwoService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            var mail = _configuration["EmailSettings:Sender"];
            var pw = _configuration["EmailSettings:Password"];
            var smtpHost = _configuration["EmailSettings:SmtpHost"];
            var smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]!);

            var client = new SmtpClient(smtpHost, smtpPort)
            {
                EnableSsl = true,
                Credentials = new NetworkCredential(mail, pw)
            };

            return client.SendMailAsync(
                new MailMessage(from: mail!, to: email, subject, message)
            );
        }
    }
}
