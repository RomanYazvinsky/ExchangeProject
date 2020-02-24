using System.IO;
using System.Threading.Tasks;
using Exchange.Services.EmailConfirmation.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Exchange.Services.EmailConfirmation
{
    public class EmailService
    {
        private readonly IOptionsMonitor<EmailConfirmationOptions> _options;
        private const string EmailPath = "Assets/registration-confirmation-mail-form.html";
        private string? _template;

        public EmailService(IOptionsMonitor<EmailConfirmationOptions> options)
        {
            _options = options;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            var options = _options.CurrentValue;
            emailMessage.From.Add(new MailboxAddress("Администрация сайта", options.Account));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(options.SmtpServer, options.Port, SecureSocketOptions.SslOnConnect);
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            await client.AuthenticateAsync(options.Account, options.Password);
            await client.SendAsync(emailMessage);

            await client.DisconnectAsync(true);
        }

        public async Task<string> LoadCommonEmailTemplate()
        {
            if (_template == default)
            {
                // todo add lock
                _template = await File.ReadAllTextAsync(EmailPath);
            }

            return _template;
        }
    }
}
