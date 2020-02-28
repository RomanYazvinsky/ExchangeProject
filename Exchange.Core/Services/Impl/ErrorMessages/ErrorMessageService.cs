using Exchange.Core.Constants.Errors;

namespace Exchange.Core.Services.Impl.ErrorMessages
{
    public class ErrorMessageService : IErrorMessageService
    {
        public string GetErrorMessage(string key)
        {
            return key;
        }

        public string GetErrorMessage(UserValidationResult error)
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
    }
}
