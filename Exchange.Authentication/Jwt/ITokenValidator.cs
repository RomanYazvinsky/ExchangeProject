using Exchange.Authentication.Jwt.Models;

namespace Exchange.Authentication.Jwt
{
    public interface ITokenValidator
    {
        TokenValidationInfo ValidateToken(JwtOptions options, string token);
    }
}
