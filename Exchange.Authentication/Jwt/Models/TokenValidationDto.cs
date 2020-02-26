using System.Security.Claims;

namespace Exchange.Authentication.Jwt.Models
{
    public class TokenValidationDto
    {
        public AuthTokenValidationResult ValidationResult { get; set; }
        public ClaimsPrincipal? ClaimsPrincipal { get; set; }
    }
}
