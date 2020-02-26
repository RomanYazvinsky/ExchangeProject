using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Exchange.Authentication.Jwt.Models;
using Exchange.Core.Constants;
using Exchange.Data.Constants;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Exchange.Authentication.Jwt.Impl
{
    public class JwtTokenFactory : ITokenFactory
    {
        private readonly JwtOptions _options;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly SigningCredentials _signingCredentials;

        public JwtTokenFactory(
            IOptionsMonitor<JwtOptions> options,
            JwtSecurityTokenHandler tokenHandler
        )
        {
            _options = options.Get(AuthenticationConstants.JwtAuthenticationScheme);
            _tokenHandler = tokenHandler;

            var signingKeyBytes = Encoding.ASCII.GetBytes(_options.SigningKey);
            _signingCredentials = new SigningCredentials(new SymmetricSecurityKey(signingKeyBytes),
                SecurityAlgorithms.HmacSha256Signature);
        }

        public string BuildAccessToken(Guid userId, Role role)
        {
            var claims = BuildAccessUserClaims(userId, role);
            var expiration = DateTime.UtcNow + _options.AccessTokenExpiration;
            return _tokenHandler.WriteToken(BuildSecurityToken(claims, expiration));
        }

        public string BuildRefreshToken(Guid userId)
        {
            var claims = BuildRefreshUserClaims(userId);
            var expiration = DateTime.UtcNow + _options.RefreshTokenExpiration;
            return _tokenHandler.WriteToken(BuildSecurityToken(claims, expiration));
        }

        private IEnumerable<Claim> BuildRefreshUserClaims(Guid userId)
        {
            return new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            };
        }

        private IEnumerable<Claim> BuildAccessUserClaims(Guid userId, Role role)
        {
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, userId.ToString());
            var roleClaim = new Claim(ClaimTypes.Role, role.ToString());
            var authClaim = new Claim(ClaimTypes.AuthenticationMethod, AuthenticationConstants.JwtAuthenticationScheme);
            return new[] {nameClaim, roleClaim, authClaim};
        }

        private SecurityToken BuildSecurityToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var jwtOptions = _options;
            return new JwtSecurityToken(
                jwtOptions.TokenValidationParameters.ValidIssuer,
                jwtOptions.TokenValidationParameters.ValidAudience,
                claims,
                expires: expires,
                signingCredentials: _signingCredentials
            );
        }
    }
}
