using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatabaseModel;
using DatabaseModel.Entities;
using Exchange.Constants;
using Exchange.Models;
using Exchange.Services.Authentication.Options;
using Exchange.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Exchange.Services.Authentication {
    public class AuthService
    {
        private const string ClaimUserId = "UserId";
        private const string ClaimDeviceLoginId = "DeviceLoginId";
        private readonly ExchangeDbContext _context;
        private readonly IOptionsMonitor<JwtOptions> _monitor;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly ErrorMessageService _errorMessageService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            ExchangeDbContext context,
            IOptionsMonitor<JwtOptions> monitor,
            JwtSecurityTokenHandler handler,
            ErrorMessageService errorMessageService,
            ILogger<AuthService> logger
        ) {
            _context = context;
            _monitor = monitor;
            _handler = handler;
            _errorMessageService = errorMessageService;
            _logger = logger;
        }

        public async Task<UserDTO> GetCurrentUserInfo(HttpRequest request)
        {
            var userIdClaim = request.HttpContext.User.Claims.First(claim => ClaimTypes.NameIdentifier.Equals(claim.Type));
            var guid = Guid.Parse(userIdClaim.Value);
            var user = await _context.Users.FindAsync(guid);
            return new UserDTO(user);
        }

        public async Task Logout(string refreshToken) {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw _errorMessageService.BuildError(AuthErrorTypes.InvalidToken);
            }
            try
            {
                var claimsPrincipal =
                    _handler.ValidateToken(refreshToken,
                        _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme).TokenValidationParameters.Clone(), out _);
                var deviceLoginId = claimsPrincipal.Claims.FirstOrDefault(claim => ClaimDeviceLoginId.Equals(claim.Type));
                if (deviceLoginId == null)
                {
                    throw _errorMessageService.BuildError(AuthErrorTypes.InvalidToken);
                }

                var deviceLogin = await _context.UserDeviceLogins.FindAsync(Guid.Parse(deviceLoginId.Value));
                _context.UserDeviceLogins.Remove(deviceLogin);
                await _context.SaveChangesAsync();
            }
            catch (Exception e)
            {
                _logger.Log(LogLevel.Error, e.Message);
                throw;
            }
        }
        public async Task<AuthDto> Login(UserLoginModel userLoginModel, HttpRequest request)
        {
            var user = await _context.Users
                .Include(u => u.UserDeviceLogins)
                .FirstOrDefaultAsync(u => u.UserName.Equals(userLoginModel.Username));
            if (user == null)
            {
                throw _errorMessageService.BuildError(AuthErrorTypes.UserNotFound);
            }

            if (!PasswordUtil.PasswordEqual(userLoginModel.Password, user.PasswordHash))
            {
                throw _errorMessageService.BuildError(ValidationErrorTypes.InvalidPassword);
            }

            var jwtOptions = _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme);
            var refreshExpiration = DateTime.UtcNow + jwtOptions.RefreshTokenExpiration;
            var refreshClaims = await GenerateRefreshUserClaim(user, refreshExpiration, request);
            if (refreshClaims == null)
            {
                throw _errorMessageService.BuildError(AuthErrorTypes.DeviceAuthRemoved);
            }

            return GenerateTokenPair(
                user,
                GenerateAccessUserClaims(user),
                refreshClaims,
                DateTime.UtcNow + jwtOptions.AccessTokenExpiration,
                refreshExpiration
            );
        }
        public async Task<AuthDto> RefreshToken(string refreshToken, HttpRequest request)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw _errorMessageService.BuildError(AuthErrorTypes.InvalidToken);
            }

            try
            {
                var jwtOptions = _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme);
                var claimsPrincipal =
                    _handler.ValidateToken(refreshToken,
                        jwtOptions.TokenValidationParameters.Clone(),
                        out _);
                var userIdClaim = claimsPrincipal.Claims.First(claim => ClaimUserId.Equals(claim.Type));
                var deviceLoginIdClaim = claimsPrincipal.Claims.First(claim => ClaimDeviceLoginId.Equals(claim.Type));
                var guid = Guid.Parse(userIdClaim.Value);
                var user = await _context.Users
                    .Include(u => u.UserDeviceLogins)
                    .FirstOrDefaultAsync(u => u.Guid == guid);
                var refreshExpiration = DateTime.UtcNow + jwtOptions.RefreshTokenExpiration;
                var refreshClaims = await GenerateRefreshUserClaim(user, refreshExpiration, request, deviceLoginIdClaim);
                if (refreshClaims == null)
                {
                    throw _errorMessageService.BuildError(AuthErrorTypes.DeviceAuthRemoved);
                }

                return GenerateTokenPair(
                    user,
                    GenerateAccessUserClaims(user),
                    refreshClaims,
                    DateTime.UtcNow + jwtOptions.AccessTokenExpiration,
                    refreshExpiration
                );
            }
            catch (SecurityTokenExpiredException expired)
            {
                throw _errorMessageService.BuildError(AuthErrorTypes.ExpiredToken);
            }
            catch (Exception e)
            {
                throw _errorMessageService.BuildError(AuthErrorTypes.InvalidToken, e);
            }
        }


        private async Task<IEnumerable<Claim>?> GenerateRefreshUserClaim(
            UserEntity user,
            DateTime expires,
            HttpRequest request,
            Claim previousRefreshTokenClaim = null
        ) {
            if (previousRefreshTokenClaim != null) {
                var value = Guid.Parse(previousRefreshTokenClaim.Value);
                var deviceLogin = user.UserDeviceLogins?.FirstOrDefault(login => login.Guid == value);
                if (deviceLogin == null) {
                    return null;
                }

                deviceLogin.ValidUntil = expires;
                await _context.SaveChangesAsync();
                return new[] {
                    new Claim(ClaimUserId, user.Guid.ToString()),
                    new Claim(ClaimDeviceLoginId, deviceLogin.Guid.ToString()),
                };
            }

            var userDeviceLogin = new UserDeviceLoginEntity {
                User = user,
                ValidUntil = expires,
                DeviceInfo = request.Headers[HeaderNames.UserAgent]
            };
            await _context.UserDeviceLogins.AddAsync(userDeviceLogin);
            await _context.SaveChangesAsync();
            return new[] {
                new Claim(ClaimUserId, user.Guid.ToString()),
                new Claim(ClaimDeviceLoginId, userDeviceLogin.Guid.ToString()),
            };
        }

        private Claim[] GenerateAccessUserClaims(UserEntity user) {
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString());
            var roleClaim = new Claim(ClaimTypes.Role, user.Role.ToString());
            var authClaim = new Claim(ClaimTypes.AuthenticationMethod, AuthenticationConstants.JwtAuthenticationScheme);
            return new[] {nameClaim, roleClaim, authClaim};
        }

        private AuthDto GenerateTokenPair(
            UserEntity user,
            IEnumerable<Claim> userClaims,
            IEnumerable<Claim> refreshClaims,
            DateTime accessExpiration,
            DateTime refreshExpiration
        ) {
            var accessTokenJson = _handler.WriteToken(BuildSecurityToken(userClaims, accessExpiration));
            var refreshTokenJson = _handler.WriteToken(BuildSecurityToken(refreshClaims, refreshExpiration));
            return new AuthDto {
                AccessToken = accessTokenJson,
                RefreshToken = refreshTokenJson,
                UserInfo = new UserDTO(user),
                ServerUtcNow = DateTime.UtcNow,
                AccessUtcValidTo = accessExpiration
            };
        }

        private JwtSecurityToken BuildSecurityToken(IEnumerable<Claim> claims, DateTime expires) {
            var jwtOptions = _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme);
            var signingKeyBytes = Encoding.ASCII.GetBytes(jwtOptions.SigningKey);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(signingKeyBytes),
                SecurityAlgorithms.HmacSha256Signature);
            return new JwtSecurityToken(
                jwtOptions.TokenValidationParameters.ValidIssuer,
                jwtOptions.TokenValidationParameters.ValidAudience,
                claims,
                expires: expires,
                signingCredentials: signingCredentials
            );
        }
    }
}
