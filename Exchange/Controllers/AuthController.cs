using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Exchange.Constants;
using Exchange.Entities;
using Exchange.Models;
using Exchange.Services;
using Exchange.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

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

        [HttpGet("refreshToken")]
        [AllowAnonymous]
        public async Task<AuthInfoModel> RefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                throw _errorMessageService.BuildError(ErrorTypes.InvalidToken);
            }

            try
            {
                var claimsPrincipal =
                    _handler.ValidateToken(refreshToken,
                        _monitor.Get(JwtAuthHandler.JwtAuthScheme).TokenValidationParameters.Clone(), out _);
                var userIdClaim = claimsPrincipal.Claims.First(claim => claim.Type.Equals("UserId"));
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
                    DateTime.UtcNow.AddMinutes(1),
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
        public async Task<AuthInfoModel> Login(UserLoginModel userLoginModel)
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
                DateTime.UtcNow.AddMinutes(1),
                refreshExpiration
                );
        }


        private Claim[] GenerateAccessUserClaims(User user)
        {
            var nameClaim = new Claim(ClaimTypes.NameIdentifier, user.Guid.ToString());
            var roleClaim = new Claim(ClaimTypes.Role, user.Role.ToString());
            var authClaim = new Claim(ClaimTypes.AuthenticationMethod, JwtAuthHandler.JwtAuthScheme);
            return new[] {nameClaim, roleClaim, authClaim};
        }

        private async Task<IEnumerable<Claim>?> GenerateRefreshUserClaim(
            User user,
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
                    new Claim("UserId", user.Guid.ToString()),
                    new Claim("DeviceLoginId", deviceLogin.Guid.ToString()),
                };
            }

            var userDeviceLogin = new UserDeviceLogin
            {
                User = user,
                ValidUntil = expires,
                DeviceInfo = Request.Headers["User-Agent"]
            };
            await _context.UserDeviceLogins.AddAsync(userDeviceLogin);
            await _context.SaveChangesAsync();
            return new[]
            {
                new Claim("UserId", user.Guid.ToString()),
                new Claim("DeviceLoginId", userDeviceLogin.Guid.ToString()),
            };
        }

        private AuthInfoModel GenerateTokenPair(
            User user,
            IEnumerable<Claim> userClaims,
            IEnumerable<Claim> refreshClaims,
            DateTime accessExpiration,
            DateTime refreshExpiration
        )
        {
            var accessToken = new JwtSecurityToken(
                "Exchange",
                "ExchangeApp",
                userClaims,
                expires: accessExpiration,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes("01234567012345670123456701234567")),
                    SecurityAlgorithms.HmacSha256Signature)
            );
            var refreshToken = new JwtSecurityToken(
                "Exchange",
                "ExchangeApp",
                refreshClaims,
                expires: refreshExpiration,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes("01234567012345670123456701234567")),
                    SecurityAlgorithms.HmacSha256Signature)
            );
            var accessTokenJson = _handler.WriteToken(accessToken);
            var refreshTokenJson = _handler.WriteToken(refreshToken);
            return new AuthInfoModel
            {
                AccessToken = accessTokenJson,
                RefreshToken = refreshTokenJson,
                UserInfo = new UserInfoModel(user),
                ServerUtcNow = DateTime.UtcNow,
                AccessUtcValidTo = accessExpiration
            };
        }
    }
}
