using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Controllers
{
    public class JwtOptions : AuthenticationSchemeOptions
    {
        public TokenValidationParameters TokenValidationParameters { get; set; }
    }
}