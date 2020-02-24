using System.Security.Claims;

namespace Exchange.Authentication.Jwt.Models
{
    public class TokenValidationInfo
    {
        public AuthTokenValidationResult ValidationResult { get; set; }
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }
    }
}
