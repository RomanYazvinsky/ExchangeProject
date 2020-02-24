using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Exchange.Authentication.Jwt.Impl;
using Exchange.Authentication.Jwt.Models;
using Exchange.Common.Utils;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;
using Exchange.Data;
using Microsoft.EntityFrameworkCore;

namespace Exchange.Authentication
{
    public class AuthService
    {
        private readonly ExchangeDbContext _context;
        private readonly JwtAuthTokenValidationHandler _authTokenValidationHandler;
        private readonly JwtTokenFactory _tokenFactory;

        public AuthService(
            ExchangeDbContext context,
            JwtAuthTokenValidationHandler authTokenValidationHandler,
            JwtTokenFactory tokenFactory
        )
        {
            _context = context;
            _authTokenValidationHandler = authTokenValidationHandler;
            _tokenFactory = tokenFactory;
        }

        public async Task<UserDto?> GetCurrentUserAsync(IEnumerable<Claim> claims)
        {
            var id = GetUserId(claims);
            if (id == null)
            {
                return null;
            }
            var user = await _context.Users.FindAsync(id.Value);
            return user == null ? null : new UserDto(user);
        }

        public async Task<AuthInfo> LoginAsync([NotNull] string username, [NotNull] string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.Equals(username));
            if (user == null)
            {
                return new AuthInfo
                {
                    Result = AuthValidationResult.UserNotFound
                };
            }

            if (!PasswordUtil.PasswordEqual(password, user.PasswordHash))
            {
                return new AuthInfo
                {
                    Result = AuthValidationResult.InvalidPassword
                };
            }

            return new AuthInfo
            {
                Auth = new AuthDto
                {
                    AccessToken = _tokenFactory.BuildAccessToken(user.Id, user.Role),
                    RefreshToken = _tokenFactory.BuildRefreshToken(user.Id)
                }
            };
        }

        public async Task<AuthInfo> RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new AuthInfo
                {
                    Result = AuthValidationResult.InvalidRefreshToken
                };
            }

            var validationResult = _authTokenValidationHandler.ValidateToken(refreshToken);
            if (validationResult.ValidationResult != AuthTokenValidationResult.Ok)
            {
                if (validationResult.ValidationResult == AuthTokenValidationResult.Expired)
                {
                    return new AuthInfo
                    {
                        Result = AuthValidationResult.ExpiredRefreshToken
                    };
                }

                return new AuthInfo
                {
                    Result = AuthValidationResult.InvalidRefreshToken
                };
            }

            var guid =  GetUserId(validationResult.ClaimsPrincipal.Claims);
            var user = await _context.Users.FindAsync(guid);
            return new AuthInfo
            {
                Auth = new AuthDto
                {
                    AccessToken = _tokenFactory.BuildAccessToken(user.Id, user.Role),
                    RefreshToken = _tokenFactory.BuildRefreshToken(user.Id)
                }
            };
        }

        private Guid? GetUserId(IEnumerable<Claim> claims)
        {
            var idClaim = claims.FirstOrDefault(claim => ClaimTypes.NameIdentifier.Equals(claim.Type));
            if (idClaim == null)
            {
                return null;
            }
            return Guid.Parse(idClaim.Value);
        }
    }
}
