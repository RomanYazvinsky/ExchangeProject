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
using Exchange.Services;
using Exchange.Services.Authentication;
using Exchange.Services.Authentication.Options;
using Exchange.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;

namespace Exchange.Controllers
{
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ExchangeDbContext _context;
        private readonly JwtSecurityTokenHandler _handler;
        private readonly IOptionsMonitor<JwtOptions> _monitor;
        private readonly ErrorMessageService _errorMessageService;

        public AuthController(
            ExchangeDbContext context,
            JwtSecurityTokenHandler handler,
            IOptionsMonitor<JwtOptions> monitor,
            ErrorMessageService errorMessageService)
        {
            _context = context;
            _handler = handler;
            _monitor = monitor;
            _errorMessageService = errorMessageService;
        }

        [HttpGet("username")]
        [AllowAnonymous]
        public Task<bool> CheckExist(string username)
        {
            return _context.Users.AnyAsync(user => user.UserName.Equals(username));
        }

        [HttpGet("logout")]
        [Authorize]
        public async Task Logout(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw _errorMessageService.BuildError(ErrorTypes.InvalidToken);
            }

            try
            {
                var claimsPrincipal =
                    _handler.ValidateToken(refreshToken,
                        _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme).TokenValidationParameters.Clone(), out _);
                var deviceLoginId = claimsPrincipal.Claims.FirstOrDefault(claim => claim.Type.Equals("DeviceLoginId"));
                if (deviceLoginId == null)
                {
                    throw _errorMessageService.BuildError(ErrorTypes.InvalidToken);
                }

                var deviceLogin = await _context.UserDeviceLogins.FindAsync(Guid.Parse(deviceLoginId.Value));
                _context.UserDeviceLogins.Remove(deviceLogin);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        [HttpGet("refreshToken")]
        [AllowAnonymous]
        public async Task<AuthDto> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw _errorMessageService.BuildError(ErrorTypes.InvalidToken);
            }

            try
            {
                var claimsPrincipal =
                    _handler.ValidateToken(refreshToken,
                        _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme).TokenValidationParameters.Clone(), out _);
                var userIdClaim = Request.HttpContext.User.Claims.First(claim => claim.Type.Equals("UserId"));
                var deviceLoginIdClaim = claimsPrincipal.Claims.First(claim => claim.Type.Equals("DeviceLoginId"));
                var guid = Guid.Parse(userIdClaim.Value);
                var user = await _context.Users
                    .Include(u => u.UserDeviceLogins)
                    .FirstOrDefaultAsync(u => u.Guid == guid);
                var refreshExpiration = DateTime.UtcNow.AddDays(30);
                var refreshClaims = await GenerateRefreshUserClaim(user, refreshExpiration, deviceLoginIdClaim);
                if (refreshClaims == null)
                {
                    throw _errorMessageService.BuildError(ErrorTypes.DeviceAuthRemoved);
                }

                return GenerateTokenPair(
                    user,
                    GenerateAccessUserClaims(user),
                    refreshClaims,
                    DateTime.UtcNow.AddMinutes(10),
                    refreshExpiration
                );
            }
            catch (SecurityTokenExpiredException expired)
            {
                throw _errorMessageService.BuildError(ErrorTypes.ExpiredToken);
            }
            catch (Exception e)
            {
                throw _errorMessageService.BuildError(ErrorTypes.InvalidToken);
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<AuthDto> Login(UserLoginModel userLoginModel)
        {
            var user = await _context.Users
                .Include(u => u.UserDeviceLogins)
                .FirstOrDefaultAsync(u => u.UserName.Equals(userLoginModel.Username));
            if (user == null)
            {
                // todo log invalid login
                return null;
            }

            if (!PasswordUtil.PasswordEqual(userLoginModel.Password, user.PasswordHash))
            {
                // todo log invalid password
                return null;
            }

            var refreshExpiration = DateTime.UtcNow.AddDays(30);
            var refreshClaims = await GenerateRefreshUserClaim(user, refreshExpiration);
            if (refreshClaims == null)
            {
                throw _errorMessageService.BuildError(ErrorTypes.DeviceAuthRemoved);
            }

            return GenerateTokenPair(
                user,
                GenerateAccessUserClaims(user),
                refreshClaims,
                DateTime.UtcNow.AddMinutes(10),
                refreshExpiration
            );
        }


        private Claim[] GenerateAccessUserClaims(UserEntity user)
        {
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString());
            var roleClaim = new Claim(ClaimTypes.Role, user.Role.ToString());
            var authClaim = new Claim(ClaimTypes.AuthenticationMethod, AuthenticationConstants.JwtAuthenticationScheme);
            return new[] {nameClaim, roleClaim, authClaim};
        }

        private async Task<IEnumerable<Claim>?> GenerateRefreshUserClaim(
            UserEntity user,
            DateTime expires,
            Claim previousRefreshTokenClaim = null
        )
        {
            if (previousRefreshTokenClaim != null)
            {
                var value = Guid.Parse(previousRefreshTokenClaim.Value);
                var deviceLogin = user.UserDeviceLogins?.FirstOrDefault(login => login.Guid == value);
                if (deviceLogin == null)
                {
                    return null;
                }

                deviceLogin.ValidUntil = expires;
                await _context.SaveChangesAsync();
                return new[]
                {
                    new Claim("DeviceLoginId", deviceLogin.Guid.ToString()),
                };
            }

            var userDeviceLogin = new UserDeviceLoginEntity
            {
                User = user,
                ValidUntil = expires,
                DeviceInfo = Request.Headers[HeaderNames.UserAgent]
            };
            await _context.UserDeviceLogins.AddAsync(userDeviceLogin);
            await _context.SaveChangesAsync();
            return new[]
            {
                new Claim("DeviceLoginId", userDeviceLogin.Guid.ToString()),
            };
        }

        private AuthDto GenerateTokenPair(
            UserEntity user,
            IEnumerable<Claim> userClaims,
            IEnumerable<Claim> refreshClaims,
            DateTime accessExpiration,
            DateTime refreshExpiration
        )
        {
            var accessTokenJson = _handler.WriteToken(BuildSecurityToken(userClaims, accessExpiration));
            var refreshTokenJson = _handler.WriteToken(BuildSecurityToken(refreshClaims, refreshExpiration));
            return new AuthDto
            {
                AccessToken = accessTokenJson,
                RefreshToken = refreshTokenJson,
                UserInfo = new UserDTO(user),
                ServerUtcNow = DateTime.UtcNow,
                AccessUtcValidTo = accessExpiration
            };
        }

        private JwtSecurityToken BuildSecurityToken(IEnumerable<Claim> claims, DateTime expires)
        {
            var jwtOptions = _monitor.Get(AuthenticationConstants.JwtAuthenticationScheme);
            var signingKeyBytes = Encoding.ASCII.GetBytes(jwtOptions.SingingKey);
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
