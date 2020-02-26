using System.Collections.Generic;
using System.Threading.Tasks;
using Exchange.Authentication;
using Exchange.Authentication.Jwt;
using Exchange.Core.Constants;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;
using Exchange.Core.Services;
using Exchange.Core.Services.ErrorMessages;
using Exchange.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;
        private readonly IAuthService _authService;
        private readonly ErrorMessageService _ems;

        public UserController(
            UserService userService,
            IAuthService authService,
            ErrorMessageService ems
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

        [HttpPost("userRole")]
        [Authorize(Roles = RoleRestrictions.OnlyAdmin)]
        public async Task<IActionResult> ModifyUserRoleAsync(UserVm userVm)
        {
            var modified = await _userService.ModifyUserRoleAsync(userVm.Id, userVm.Role);
            if (!modified)
            {
                return BadRequest(_ems.GetErrorMessage(AuthValidationResult.UserNotFound));
            }
            return Ok();
        }
    }
}
