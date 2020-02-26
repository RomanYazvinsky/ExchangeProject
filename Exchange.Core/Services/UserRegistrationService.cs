using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Exchange.Common.Utils;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;
using Exchange.Core.Services.EmailConfirmation;
using Exchange.Core.Services.EmailConfirmation.Models;
using Exchange.Core.Services.ErrorMessages;
using Exchange.Core.ViewModels;
using Exchange.Data;
using Exchange.Data.Constants;
using Exchange.Data.Entities.User;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Exchange.Core.Services
{
    public class UserRegistrationService
    {
        private readonly CredentialValidationService _credentialValidationService;
        private readonly ErrorMessageService _ems;
        private readonly EmailService _emailService;
        private readonly ExchangeDbContext _context;
        private readonly ILogger<UserRegistrationService> _logger;

        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public UserRegistrationService(
            CredentialValidationService credentialValidationService,
            ErrorMessageService ems,
            EmailService emailService,
            ExchangeDbContext context,
            ILogger<UserRegistrationService> logger
        )
        {
            _credentialValidationService = credentialValidationService;
            _ems = ems;
            _emailService = emailService;
            _context = context;
            _logger = logger;
        }

        private async Task<bool> IsValid(string username, string password, string email)
        {
            var usernameErrors = await _credentialValidationService.ValidateUsernameAsync(username);
            if (usernameErrors != null)
            {
                _logger.Log(LogLevel.Error, _ems.GetErrorMessage(usernameErrors.FirstOrDefault()));
                return false;
            }

            var passwordErrors = _credentialValidationService.ValidatePassword(password);
            if (passwordErrors != null)
            {
                _logger.Log(LogLevel.Error, _ems.GetErrorMessage(passwordErrors.FirstOrDefault()));
                return false;
            }

            var emailErrors = await _credentialValidationService.ValidateEmailAsync(email);
            if (emailErrors != null)
            {
                _logger.Log(LogLevel.Error, _ems.GetErrorMessage(emailErrors.FirstOrDefault()));
                return false;
            }

            return true;
        }

        public async Task<UserVm?> RegisterUser(string username, string password, string email)
        {
            if (!await IsValid(username, password, email))
            {
                return null;
            }

            var newUser = new UserEntity
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = PasswordUtil.HashPassword(password),
                Role = Role.Customer,
                Email = email,
                IsEmailConfirmed = false
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return new UserVm(newUser);
        }

        public async Task<MailConfirmationResult> SendConfirmationEmail([NotNull] UserVm user)
        {
            if (user.IsEmailConfirmed)
            {
                return MailConfirmationResult.AlreadyConfirmed;
            }

            var confirmationId = GenerateConfirmationId(user);

            var mail = await _emailService.ComposeConfirmationEmailAsync(confirmationId);

            var sendEmailResult = await _emailService.SendEmailAsync(user.Email, "Admin", "Confirmation", mail);
            return sendEmailResult == SendEmailResult.Ok
                ? MailConfirmationResult.Ok
                : MailConfirmationResult.EmailConfirmationServiceUnavailable;
        }

        public async Task<MailConfirmationResult> ConfirmEmail(string userIdString)
        {
            if (string.IsNullOrWhiteSpace(userIdString) || !Guid.TryParse(userIdString, out var userId))
            {
                return MailConfirmationResult.InvalidConfirmationId;
            }

            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return MailConfirmationResult.InvalidConfirmationId;
            }

            if (user.IsEmailConfirmed)
            {
                return MailConfirmationResult.AlreadyConfirmed;
            }

            user.IsEmailConfirmed = true;
            await _context.SaveChangesAsync();
            return MailConfirmationResult.Ok;
        }

        private string GenerateConfirmationId(UserVm user)
        {
            var json = JsonConvert.SerializeObject(new EmailConfirmationVm(user), JsonSerializerSettings);
            return Convert.ToBase64String(
                Encoding.UTF8.GetBytes(json));
        }
    }
}
