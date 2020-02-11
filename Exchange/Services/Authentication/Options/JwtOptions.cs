using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Services.Authentication.Options
{
    public class SigningKeyModel
    {
        public string SigningKey { get; set; }
    }

    public class JwtOptions : AuthenticationSchemeOptions
    {
        public string SingingKey { get; set; }
        public TokenValidationParameters TokenValidationParameters { get; set; }
    }
}
