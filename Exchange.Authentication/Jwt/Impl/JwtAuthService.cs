using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Exchange.Authentication.Jwt.Models;
using Exchange.Common.Utils;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Core.ViewModels;
using Exchange.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Exchange.Authentication.Jwt.Impl
{
    public class JwtAuthService : IAuthService
    {
        private readonly ExchangeDbContext _context;
        private readonly ITokenValidator _tokenValidator;
        private readonly ITokenFactory _tokenFactory;
        private readonly JwtOptions _jwtOptions;

        public JwtAuthService(
            ExchangeDbContext context,
            IOptionsMonitor<JwtOptions> optionsMonitor,
            ITokenValidator tokenValidator,
            ITokenFactory tokenFactory
        )
        {
            _context = context;
            _jwtOptions = optionsMonitor.Get(AuthenticationConstants.JwtAuthenticationScheme);
            _tokenValidator = tokenValidator;
            _tokenFactory = tokenFactory;
        }

        public async Task<UserVm?> GetCurrentUserAsync(IEnumerable<Claim> claims)
        {
            var id = AuthUtils.GetUserId(claims);
            if (id == null)
            {
                return null;
            }

            var user = await _context.Users.FindAsync(id.Value);
            return user == null ? null : new UserVm(user);
        }

        public async Task<AuthDto> LoginAsync([NotNull] string username, [NotNull] string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username.Equals(username));
            if (user == null)
            {
                return new AuthDto
                {
                    Result = UserValidationResult.UserNotFound
                };
            }

            if (!PasswordUtil.PasswordEqual(password, user.PasswordHash))
            {
                return new AuthDto
                {
                    Result = UserValidationResult.InvalidPassword
                };
            }

            return new AuthDto
            {
                Auth = new AuthVm
                {
                    AccessToken = _tokenFactory.BuildAccessToken(user.Id, user.Role),
                    RefreshToken = _tokenFactory.BuildRefreshToken(user.Id)
                }
            };
        }

        public async Task<AuthDto> RefreshTokenAsync(string? refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return new AuthDto
                {
                    Result = UserValidationResult.InvalidRefreshToken
                };
            }

            var validationResult = _tokenValidator.ValidateToken(_jwtOptions, refreshToken);
            if (validationResult.ValidationResult != AuthTokenValidationResult.Ok)
            {
                if (validationResult.ValidationResult == AuthTokenValidationResult.Expired)
                {
                    return new AuthDto
                    {
                        Result = UserValidationResult.ExpiredRefreshToken
                    };
                }

                return new AuthDto
                {
                    Result = UserValidationResult.InvalidRefreshToken
                };
            }

            var guid = AuthUtils.GetUserId(validationResult.ClaimsPrincipal.Claims);
            var user = await _context.Users.FindAsync(guid);
            return new AuthDto
            {
                Auth = new AuthVm
                {
                    AccessToken = _tokenFactory.BuildAccessToken(user.Id, user.Role),
                    RefreshToken = _tokenFactory.BuildRefreshToken(user.Id)
                }
            };
        }
    }
}
