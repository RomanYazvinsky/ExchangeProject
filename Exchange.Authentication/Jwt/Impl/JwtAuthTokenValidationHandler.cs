using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exchange.Authentication.Jwt.Models;
using Exchange.Core.Constants;
using Exchange.Core.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Exchange.Authentication.Jwt.Impl
{
    public class JwtAuthTokenValidationHandler : AuthenticationHandler<JwtOptions>
    {
        private readonly IErrorMessageService _ems;
        private readonly ITokenValidator _tokenValidator;

        public JwtAuthTokenValidationHandler(
            IErrorMessageService ems,
            IOptionsMonitor<JwtOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            ITokenValidator tokenValidator
        ) : base(options, logger, encoder, clock)
        {
            _ems = ems;
            _tokenValidator = tokenValidator;
        }


        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.TokenValidationParameters == null)
            {
                return Task.FromResult(AuthenticateResult.Fail(_ems.GetErrorMessage("error.auth.not.configured")));
            }

            var authHeader = Request.Headers[HeaderNames.Authorization];
            var jwtBearerHeader = authHeader.FirstOrDefault(s =>
                s?.StartsWith(AuthenticationConstants.AuthenticationHeader)
                ?? false);
            if (string.IsNullOrWhiteSpace(jwtBearerHeader))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var token = jwtBearerHeader.Substring(AuthenticationConstants.AuthenticationHeader.Length).Trim();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            var tokenValidationResult = _tokenValidator.ValidateToken(Options, token);
            if (tokenValidationResult.ValidationResult != AuthTokenValidationResult.Ok)
            {
                return Task.FromResult(AuthenticateResult.Fail(_ems.GetErrorMessage("error.auth.invalid.token")));
            }

            var claimsIdentity = tokenValidationResult.ClaimsPrincipal;
            var authenticationTicket = new AuthenticationTicket(
                new ClaimsPrincipal(claimsIdentity),
                AuthenticationConstants.JwtAuthenticationScheme
            );
            return Task.FromResult(AuthenticateResult.Success(authenticationTicket));
        }

        protected override Task HandleChallengeAsync(AuthenticationProperties properties)
        {
            Response.Headers[HeaderNames.WWWAuthenticate] = AuthenticationConstants.AuthenticationHeader;
            return base.HandleChallengeAsync(properties);
        }
    }
}
