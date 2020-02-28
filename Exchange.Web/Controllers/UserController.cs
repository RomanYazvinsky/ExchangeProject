using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Authentication.Jwt;
using Exchange.Common.Utils;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Services;
using Exchange.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IErrorMessageService _ems;

        public UserController(
            IUserService userService,
            IAuthService authService,
            IErrorMessageService ems
        )
        {
            _userService = userService;
            _authService = authService;
            _ems = ems;
        }

        [HttpGet("currentUser")]
        [Authorize]
        public Task<UserVm> GetCurrentUserInfo()
        {
            return _authService.GetCurrentUserAsync(Request.HttpContext.User.Claims);
        }

        [HttpGet("users")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public Task<IEnumerable<UserVm>> GetAllUsersAsync()
        {
            return _userService.GetAllUsersAsync();
        }


        [HttpPost("removeUser")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public async Task<IActionResult> RemoveUserAsync(UserVm user)
        {
            if (AuthUtils.GetUserId(HttpContext.User.Claims) == user.Id)
            {
                return BadRequest(_ems.GetErrorMessage("error.remove.current.user"));
            }
            var result = await _userService.RemoveUserAsync(user.Id);
            if (!result)
            {
                return BadRequest(_ems.GetErrorMessage(UserValidationResult.UserNotFound));
            }

            return Ok();
        }

        [HttpPost("userRole")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public async Task<IActionResult> ModifyUserRoleAsync(UserVm userVm)
        {
            var modified = await _userService.ModifyUserRoleAsync(userVm.Id, userVm.Role);
            if (!modified)
            {
                return BadRequest(_ems.GetErrorMessage(UserValidationResult.UserNotFound));
            }
            return Ok();
        }
    }
}
