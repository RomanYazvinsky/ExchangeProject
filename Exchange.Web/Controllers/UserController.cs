using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Authentication;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;
using Exchange.Core.Services;
using Exchange.Core.Services.ErrorMessages;
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
        private readonly ErrorMessageService _ems;

        public UserController(
            UserService userService,
            AuthService authService,
            ErrorMessageService ems
        )
        {
            _userService = userService;
            _authService = authService;
            _ems = ems;
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
            var modified = await _userService.ModifyUserRoleAsync(userDto.Id, userDto.Role);
            if (!modified)
            {
                return BadRequest(_ems.GetErrorMessage(AuthValidationResult.UserNotFound));
            }
            return Ok();
        }
    }
}
