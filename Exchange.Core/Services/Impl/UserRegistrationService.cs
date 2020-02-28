using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Exchange.Common.Utils;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Services.Models;
using Exchange.Core.ViewModels;
using Exchange.Data;
using Exchange.Data.Constants;
using Exchange.Data.Entities.User;
using Microsoft.Extensions.Logging;

namespace Exchange.Core.Services.Impl
{
    public class UserRegistrationService : IUserRegistrationService
    {
        private readonly ICredentialValidationService _credentialValidationService;
        private readonly IErrorMessageService _ems;
        private readonly IEmailService _emailService;
        private readonly ExchangeDbContext _context;
        private readonly ILogger<UserRegistrationService> _logger;

        private static readonly JsonSerializerOptions JsonSerializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public UserRegistrationService(
            ICredentialValidationService credentialValidationService,
            IErrorMessageService ems,
            IEmailService emailService,
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

        private async Task<Dictionary<CredentialProperty, IEnumerable<string>>> GetValidationErrors(
            string? username,
            string? password,
            string? email
        )
        {
            var errors = new Dictionary<CredentialProperty, IEnumerable<string>>();
            var usernameErrors = await _credentialValidationService.ValidateUsernameAsync(username);
            if (usernameErrors != null)
            {
                var usernameValidationErrors = usernameErrors.ToList();
                _logger.Log(LogLevel.Error, _ems.GetErrorMessage(usernameValidationErrors.FirstOrDefault()));
                errors.Add(
                    CredentialProperty.Username,
                    usernameValidationErrors.Select(error => _ems.GetErrorMessage(error))
                );
            }

            var passwordErrors = _credentialValidationService.ValidatePassword(password);
            if (passwordErrors != null)
            {
                var passwordValidationErrors = passwordErrors.ToList();
                _logger.Log(LogLevel.Error, _ems.GetErrorMessage(passwordValidationErrors.FirstOrDefault()));
                errors.Add(
                    CredentialProperty.Password,
                    passwordValidationErrors.Select(error => _ems.GetErrorMessage(error))
                );
            }

            var emailErrors = await _credentialValidationService.ValidateEmailAsync(email);
            if (emailErrors != null)
            {
                var emailValidationErrors = emailErrors.ToList();
                _logger.Log(LogLevel.Error, _ems.GetErrorMessage(emailValidationErrors.FirstOrDefault()));
                errors.Add(
                    CredentialProperty.Email,
                    emailValidationErrors.Select(error => _ems.GetErrorMessage(error))
                );
            }

            return errors;
        }

        public async Task<(UserVm? User, Dictionary<CredentialProperty, IEnumerable<string>> ErrorMessages)>
            RegisterUser(
                string? username,
                string? password,
                string? email
            )
        {
            var errors = await GetValidationErrors(username, password, email);
            if (errors.Any())
            {
                return (null, errors);
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
            return (new UserVm(newUser), errors);
        }

        public MailConfirmationResult SendConfirmationEmail([NotNull] UserVm user)
        {
            if (user.IsEmailConfirmed)
            {
                return MailConfirmationResult.AlreadyConfirmed;
            }

            var confirmationId = GenerateConfirmationId(new EmailConfirmationVm(user));

            var mail = _emailService.ComposeConfirmationEmail(confirmationId);

            _emailService.ScheduleEmail(user.Email, "Admin", "Confirmation", mail);
            return MailConfirmationResult.Ok;
        }

        public async Task<MailConfirmationResult> ConfirmEmail(string? userIdString)
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

        private string GenerateConfirmationId([NotNull] EmailConfirmationVm user)
        {
            var json = JsonSerializer.Serialize(user, JsonSerializerOptions);
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }
    }
}
