using Exchange.Core.Constants.Errors;
using Exchange.Core.ViewModels;

namespace Exchange.Authentication.Jwt.Models
{
    public class AuthDto
    {
        public AuthVm? Auth { get; set; }
        public UserValidationResult Result { get; set; } = UserValidationResult.Ok;
    }
}
