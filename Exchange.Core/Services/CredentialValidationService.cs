using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Data;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Core.Services
{
    public class CredentialValidationService
    {
        private readonly ExchangeDbContext _context;

        public CredentialValidationService(ExchangeDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<UsernameValidationErrors>?> ValidateUsernameAsync(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                return new[] {UsernameValidationErrors.Empty};
            }

            var errors = new List<UsernameValidationErrors>();
            if (username.Length < CredentialsValidationConstants.MinimalUsernameLength)
            {
                errors.Add(UsernameValidationErrors.TooShort);
            }

            if (!Regex.IsMatch(username, CredentialsValidationConstants.UsernameRegex))
            {
                errors.Add(UsernameValidationErrors.ContainsUnsupportedSymbols);
            }

            if (errors.Count > 0)
            {
                return errors;
            }
            var alreadyExist = await _context.Users.AnyAsync(u => u.Username.Equals(username));
            if (alreadyExist)
            {
                return new[] {UsernameValidationErrors.AlreadyExist};
            }

            return null;
        }
        public IEnumerable<PasswordValidationErrors>? ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
            {
                return new []{PasswordValidationErrors.Empty};
            }

            if (password.Length < CredentialsValidationConstants.MinimalPasswordLength)
            {
                return new[] {PasswordValidationErrors.TooShort};
            }
            return null;
        }

        public async Task<IEnumerable<EmailValidationErrors>?> ValidateEmailAsync(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return new []{EmailValidationErrors.Empty};
            }

            if (!Regex.IsMatch(email, CredentialsValidationConstants.EmailRegex))
            {
                return new[] {EmailValidationErrors.NotMatch};
            }

            if (await _context.Users.AnyAsync(user => user.Email.Equals(email)))
            {
                return new[] {EmailValidationErrors.AlreadyUsed};
            }
            return null;
        }
    }
}
