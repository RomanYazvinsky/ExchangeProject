using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exchange.Constants;
using Exchange.Services.Authentication.Options;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Exchange.Services.Authentication
{
    public class JwtAuthHandler : AuthenticationHandler<JwtOptions>
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtAuthHandler(IOptionsMonitor<JwtOptions> options, ILoggerFactory logger, UrlEncoder encoder,
            ISystemClock clock, JwtSecurityTokenHandler tokenHandler)
            : base(options, logger, encoder, clock)
        {
            _tokenHandler = tokenHandler;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.TokenValidationParameters == null)
            {
                return AuthenticateResult.Fail("Not configured");
            }

            var authHeader = Request.Headers[HeaderNames.Authorization];
            var jwtBearerHeader = authHeader.FirstOrDefault(s =>
                s?.StartsWith(AuthenticationConstants.AuthenticationHeader)
                ?? false);
            if (string.IsNullOrWhiteSpace(jwtBearerHeader))
            {
                return AuthenticateResult.NoResult();
            }

            var token = jwtBearerHeader.Substring(AuthenticationConstants.AuthenticationHeader.Length).Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                return AuthenticateResult.NoResult();
            }

            try
            {
                var tokenValidationResult =
                    _tokenHandler.ValidateToken(token, Options.TokenValidationParameters.Clone(), out _);
                var claimsIdentity = tokenValidationResult;
                var authenticationTicket = new AuthenticationTicket(
                    new ClaimsPrincipal(claimsIdentity),
                    AuthenticationConstants.JwtAuthenticationScheme
                );
                return AuthenticateResult.Success(authenticationTicket);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return AuthenticateResult.Fail("Invalid token");
            }
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers["WWW-Authenticate"] = AuthenticationConstants.AuthenticationHeader;
            return base.HandleChallengeAsync(properties);
        }
    }
}
