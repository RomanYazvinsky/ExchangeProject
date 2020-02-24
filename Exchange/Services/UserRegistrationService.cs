using System;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DatabaseModel;
using DatabaseModel.Constants;
using DatabaseModel.Entities;
using Exchange.Constants;
using Exchange.Services.EmailConfirmation;
using Exchange.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Exchange.Services
{
    public class UserRegistrationService
    {

        /// <summary>
        /// http://emailregex.com/
        /// </summary>
        private const string EmailRegex = "^(?(\")(\".+?(?<!\\\\)\"@)|(([0-9a-z]((\\.(?!\\.))|[-!#\\$%&'\\*\\+/=\\?\\^`\\{\\}\\|~\\w])*)(?<=[0-9a-z])@))(?(\\[)(\\[(\\d{1,3}\\.){3}\\d{1,3}\\])|(([0-9a-z][-\\w]*[0-9a-z]*\\.)+[a-z0-9][\\-a-z0-9]{0,22}[a-z0-9]))$";

        private readonly EmailService _emailService;
        private readonly ExchangeDbContext _context;
        private readonly ErrorMessageService _ems;
        private readonly ILogger<UserRegistrationService> _logger;

        public UserRegistrationService(
            EmailService emailService,
            ExchangeDbContext context,
            ErrorMessageService ems,
            ILogger<UserRegistrationService> logger
        )
        {
            _emailService = emailService;
            _context = context;
            _ems = ems;
            _logger = logger;
        }

        public async Task<UserEntity> RegisterUser(
            string? username, string? password, string? email)
        {
            await ValidateUsername(username);
            var newUser = new UserEntity
            {
                Guid = Guid.NewGuid(),
                UserName = username,
                PasswordHash = PasswordUtil.HashPassword(password),
                Role = Role.Customer,
                Email = email,
                IsEmailConfirmed = false
            };
            await _context.Users.AddAsync(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        public async Task<ValidationErrorTypes?> ValidateUsername(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return ValidationErrorTypes.UserAlreadyExist;
            }

            var alreadyExist = await _context.Users.AnyAsync(u => u.UserName.Equals(username));
            if (alreadyExist)
            {
                return ValidationErrorTypes.UserAlreadyExist;
            }

            return null;
        }

        public ValidationErrorTypes? ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return ValidationErrorTypes.InvalidPassword;
            }

            return null;
        }

        public ValidationErrorTypes? ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email) || !Regex.IsMatch(email, EmailRegex))
            {
                return ValidationErrorTypes.InvalidEmail;
            }

            return null;
        }

        public async Task<MailConfirmationErrorTypes?> SendConfirmationEmail([NotNull] string confirmationHandlerUrl, [NotNull] UserEntity user)
        {
            if (user.IsEmailConfirmed)
            {
                return MailConfirmationErrorTypes.AlreadyConfirmed;
            }

            var confirmationId = GenerateConfirmationId(user);
            var confirmationLink = GenerateConfirmationLink(confirmationHandlerUrl, confirmationId);

            var template = await _emailService.LoadCommonEmailTemplate();
            var email = string.Format(template, confirmationLink);
            try
            {
                await _emailService.SendEmailAsync(user.Email, "Confirmation", email);
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Critical, e.Message, e);
                return MailConfirmationErrorTypes.EmailConfirmationServiceUnavailable;
            }
            return null;
        }

        public async Task<MailConfirmationErrorTypes?> ConfirmEmail(string confirmationId)
        {
            if (string.IsNullOrWhiteSpace(confirmationId) || !Guid.TryParse(confirmationId, out var userId))
            {
                return MailConfirmationErrorTypes.InvalidConfirmationId;
            }
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return MailConfirmationErrorTypes.InvalidConfirmationId;
            }

            user.IsEmailConfirmed = true;
            return null;
        }

        private string GenerateConfirmationLink(string clientConfirmationHandlerUrl, string confirmationId)
        {
            return $"{clientConfirmationHandlerUrl}/{confirmationId}";
        }

        private string GenerateConfirmationId(UserEntity user)
        {
            return user.Guid.ToString();
        }
    }
}
