using System;
using Exchange.Constants;

namespace Exchange.Services
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
        // todo localization service
        public string GetErrorMessage(string key)
        {
            return key;
        }

        public string GetErrorMessage(AuthErrorTypes error)
        {
            return $"[{error}]: {GetErrorMessage($"error.auth.{error}")}";
        }

        public string GetErrorMessage(ValidationErrorTypes error)
        {
            return $"[{error}]: {GetErrorMessage($"error.auth.{error}")}";
        }

        public string GetErrorMessage(MailConfirmationErrorTypes error)
        {
            return $"[{error}]: {GetErrorMessage($"error.auth.{error}")}";
        }

        public LocalizedError BuildError(string localizationKey, Exception innerException = null)
        {
            return new LocalizedError(GetErrorMessage(localizationKey), innerException);
        }
        public LocalizedError BuildError(AuthErrorTypes authError, Exception innerException = null)
        {
            return BuildError(GetErrorMessage(authError), innerException);
        }
        public LocalizedError BuildError(ValidationErrorTypes error, Exception innerException = null)
        {
            return BuildError(GetErrorMessage(error), innerException);
        }
    }
}
