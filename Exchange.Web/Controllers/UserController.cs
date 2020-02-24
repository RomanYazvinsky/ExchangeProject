using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Exchange.Authentication;
using Exchange.Core.Constants;
using Exchange.Core.Models.Dto;
using Exchange.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly AuthService _authService;

        public UserController(
            UserService userService,
            AuthService authService
        )
        {
            _userService = userService;
            _authService = authService;
        }

        [HttpGet("currentUser")]
        [Authorize]
        public Task<UserDto> GetCurrentUserInfo()
        {
            return _authService.GetCurrentUserAsync(Request.HttpContext.User.Claims);
        }

        [HttpGet("users")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            return _userService.GetAllUsersAsync();
        }

        [HttpPost("userRole")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public async Task<IActionResult> ModifyUserRoleAsync(UserDto userDto)
        {
            await _userService.ModifyUserRoleAsync(userDto.Id, userDto.Role);
            return Ok();
        }
    }
}
