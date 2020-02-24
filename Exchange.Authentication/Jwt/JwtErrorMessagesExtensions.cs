using Exchange.Authentication.Jwt.Models;
using Exchange.Core.Services.ErrorMessages;

namespace Exchange.Authentication.Jwt
{
    public static class JwtErrorMessagesExtensions
    {
        public static string GetErrorMessage(this ErrorMessageService service, AuthTokenValidationResult result)
        {
            return $"[{result}]: {service.GetErrorMessage($"error.auth.token.{result}")}";
        }
    }
}
