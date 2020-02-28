using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Exchange.Core.Services.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Exchange.Core.Services.Impl.EmailConfirmation
{
    public class EmailSenderService : BackgroundService, IEmailSenderService
    {
        private const string OAuth2Scheme = "XOAUTH2";
        private const int DefaultEmailPoolingTimeoutMilliseconds = 30000;
        private readonly IEmailService _emailService;
        private readonly ILogger<EmailSenderService> _logger;
        private readonly EmailConfirmationOptions _options;

        public EmailSenderService(
            IEmailService emailService,
            IOptions<EmailConfirmationOptions> options,
            ILogger<EmailSenderService> logger
        )
        {
            _emailService = emailService;
            _logger = logger;
            _options = options.Value;
        }

        private async Task SendEmailAsync()
        {
            var mimeMessages = _emailService.ResetQueue();
            var messages = mimeMessages.ToList();
            if (!messages.Any())
            {
                return;
            }
            using var client = new SmtpClient();

            try
            {
                await client.ConnectAsync(_options.SmtpServer, _options.Port, SecureSocketOptions.SslOnConnect);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
            }

            try
            {
                await client.AuthenticateAsync(_options.Account, _options.Password);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
            }

            client.AuthenticationMechanisms.Remove(OAuth2Scheme);

            try
            {
                Task.WaitAll(messages.Select(message => client.SendAsync(message)).ToArray());
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message);
            }

            await client.DisconnectAsync(true);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await SendEmailAsync();
                await Task.Delay(_options.EmailPoolingTimeoutMilliseconds ?? DefaultEmailPoolingTimeoutMilliseconds, stoppingToken);
            }
        }
    }
}
