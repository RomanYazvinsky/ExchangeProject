using System.Diagnostics.CodeAnalysis;
using Exchange.Core.Constants.Errors;

namespace Exchange.Core.Services
{
    public interface IErrorMessageService
    {
        string GetErrorMessage([NotNull] string key);
        string GetErrorMessage(UserValidationResult error);
        string GetErrorMessage(UsernameValidationErrors error);
        string GetErrorMessage(PasswordValidationErrors error);
        string GetErrorMessage(EmailValidationErrors error);
        string GetErrorMessage(MailConfirmationResult error);
    }
}
