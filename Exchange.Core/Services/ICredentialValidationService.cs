using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Core.Constants.Errors;

namespace Exchange.Core.Services
{
    public interface ICredentialValidationService
    {
        Task<IEnumerable<UsernameValidationErrors>?> ValidateUsernameAsync(string? username);
        IEnumerable<PasswordValidationErrors>? ValidatePassword(string? password);
        Task<IEnumerable<EmailValidationErrors>?> ValidateEmailAsync(string? email);
    }
}