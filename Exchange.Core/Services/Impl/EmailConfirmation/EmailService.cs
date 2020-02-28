using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Exchange.Core.Services.Models;
using Exchange.Core.Services.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using MimeKit.Text;

namespace Exchange.Core.Services.Impl.EmailConfirmation
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailConfirmationOptions _options;
        private readonly IList<MimeMessage> _messages = new List<MimeMessage>();
        private string? _template;

        public EmailService(IOptions<EmailConfirmationOptions> options, ILogger<EmailService> logger)
        {
            _logger = logger;
            _options = options.Value;
            _template = File.ReadAllText(_options.EmailTemplatePath);
        }

        public IEnumerable<MimeMessage> ResetQueue()
        {
            var messages = new List<MimeMessage>(_messages);
            _messages.Clear();
            return messages;
        }


        public void ScheduleEmail(string email, string sender, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(sender, _options.Account));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };
            _messages.Add(emailMessage);
        }

        public string ComposeConfirmationEmail(string confirmationId)
        {
            var url = _options.ConfirmationUrl;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            return string.Format(_template, url + confirmationId);
        }
    }
}
