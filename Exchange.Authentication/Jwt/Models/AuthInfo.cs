using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;

namespace Exchange.Authentication.Jwt.Models
{
    public class AuthInfo
    {
        public AuthDto? Auth { get; set; }
        public AuthValidationResult Result { get; set; } = AuthValidationResult.Ok;
    }
}
