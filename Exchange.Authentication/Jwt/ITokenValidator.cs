using Exchange.Authentication.Jwt.Models;

namespace Exchange.Authentication.Jwt
{
    public interface ITokenValidator
    {
        TokenValidationDto ValidateToken(JwtOptions options, string token);
    }
}
