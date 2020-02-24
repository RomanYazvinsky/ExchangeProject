using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Core.Services.EmailConfirmation.Models;
using Exchange.Core.Services.EmailConfirmation.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Exchange.Core.Services.EmailConfirmation
{
    public class EmailService
    {
        private const string OAuth2Scheme = "XOAUTH2";
        private readonly ILogger<EmailService> _logger;
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly EmailConfirmationOptions _options;
        private string? _template;

        public EmailService(IOptions<EmailConfirmationOptions> options, ILogger<EmailService> logger)
        {
            _logger = logger;
            _options = options.Value;
        }

        public async Task<SendEmailResult> SendEmailAsync(string email, string sender, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(sender, _options.Account));
            emailMessage.To.Add(new MailboxAddress("", email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };
            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_options.SmtpServer, _options.Port, SecureSocketOptions.SslOnConnect);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
                return SendEmailResult.ConnectFailed;
            }
            try
            {
                await client.AuthenticateAsync(_options.Account, _options.Password);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
                return SendEmailResult.AuthFailed;
            }

            client.AuthenticationMechanisms.Remove(OAuth2Scheme);
            try
            {
                await client.SendAsync(emailMessage);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
                return SendEmailResult.SendFailed;
            }

            await client.DisconnectAsync(true);
            return SendEmailResult.Ok;
        }

        public async Task<string> ComposeConfirmationEmailAsync(string confirmationId)
        {
            var url = _options.ConfirmationUrl;
            if (!url.EndsWith("/"))
            {
                url += "/";
            }

            var template = await LoadCommonEmailTemplateAsync();
            return string.Format(template, url + confirmationId);
        }

        private async Task<string> LoadCommonEmailTemplateAsync()
        {
            if (_template != default)
            {
                return _template;
            }

            try
            {
                await _semaphore.WaitAsync();
                _template = await File.ReadAllTextAsync(_options.EmailTemplatePath);
            }
            finally
            {
                _semaphore.Release();
            }

            return _template;
        }
    }
}
