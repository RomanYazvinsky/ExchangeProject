using System;
using Exchange.Core.Constants.Errors;

namespace Exchange.Core.Services.ErrorMessages
{
    public class LocalizedError : Exception
    {
        public LocalizedError(string localizedMessage, Exception innerException) : base(
            innerException == null
                ? localizedMessage
                : localizedMessage + Environment.NewLine + innerException.Message,
            innerException)
        {
        }
        public LocalizedError(string localizedMessage) : base(localizedMessage)
        {
        }
    }

    public class ErrorMessageService
    {
        public string GetErrorMessage(string key)
        {
            return key;
        }

        public string GetErrorMessage(AuthValidationResult error)
        {
            return GetErrorMessage($"error.auth.{error}");
        }
        public string GetErrorMessage(UsernameValidationErrors error)
        {
            return GetErrorMessage($"error.username.{error}");
        }
        public string GetErrorMessage(PasswordValidationErrors error)
        {
            return GetErrorMessage($"error.password.{error}");
        }

        public string GetErrorMessage(EmailValidationErrors error)
        {
            return GetErrorMessage($"error.email.{error}");
        }

        public string GetErrorMessage(MailConfirmationResult error)
        {
            return GetErrorMessage($"error.auth.{error}");
        }

        public LocalizedError BuildError(string localizationKey, Exception innerException = null)
        {
            return new LocalizedError(GetErrorMessage(localizationKey), innerException);
        }
        public LocalizedError BuildError(AuthValidationResult authError, Exception innerException = null)
        {
            return BuildError(GetErrorMessage(authError), innerException);
        }
    }
}
