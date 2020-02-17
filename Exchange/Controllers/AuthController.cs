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
        private readonly AuthService _authService;

        public AuthController(
            ExchangeDbContext context,
            AuthService authService
        )
        {
            _context = context;
            _authService = authService;
        }

        [HttpGet("username")]
        [AllowAnonymous]
        public Task<bool> CheckExist(string username)
        {
            return _context.Users.AnyAsync(user => user.UserName.Equals(username));
        }

        [HttpGet("logout")]
        [Authorize]
        public Task Logout(string refreshToken)
        {
            return _authService.Logout(refreshToken);
        }

        [HttpGet("refreshToken")]
        [AllowAnonymous]
        public Task<AuthDto> RefreshToken(string refreshToken)
        {
            return _authService.RefreshToken(refreshToken, Request);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public Task<AuthDto> Login(UserLoginModel userLoginModel)
        {
            return _authService.Login(userLoginModel, Request);
        }
    }
}
