using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Exchange.Authentication.Jwt.Impl.Options;
using Exchange.Authentication.Jwt.Models;
using Exchange.Core.Constants;
using Exchange.Core.Services.ErrorMessages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Exchange.Authentication.Jwt.Impl
{
    public class JwtAuthTokenValidationHandler : AuthenticationHandler<JwtOptions>
    {
        private readonly ErrorMessageService _ems;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public JwtAuthTokenValidationHandler(
            ErrorMessageService ems,
            IOptionsMonitor<JwtOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            JwtSecurityTokenHandler tokenHandler
        ): base(options, logger, encoder, clock)
        {
            _ems = ems;
            _tokenHandler = tokenHandler;
        }

        public TokenValidationInfo ValidateToken(string token)
        {
            try
            {
                var claimsPrincipal =
                    _tokenHandler.ValidateToken(token, Options.TokenValidationParameters.Clone(), out _);
                return new TokenValidationInfo
                {
                    ValidationResult = AuthTokenValidationResult.Ok,
                    ClaimsPrincipal = claimsPrincipal
                };
            }
            catch (SecurityTokenExpiredException)
            {
                return new TokenValidationInfo
                {
                    ValidationResult = AuthTokenValidationResult.Expired
                };
            }
            catch (Exception)
            {
                return new TokenValidationInfo
                {
                    ValidationResult = AuthTokenValidationResult.Corrupted
                };
            }
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

            var tokenValidationResult = ValidateToken(token);
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
