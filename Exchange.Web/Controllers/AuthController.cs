using System.Threading.Tasks;
using Exchange.Authentication.Jwt;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Services;
using Exchange.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IErrorMessageService _ems;

        public AuthController(IAuthService authService, IErrorMessageService ems)
        {
            _authService = authService;
            _ems = ems;
        }

        [HttpGet("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result.Result != UserValidationResult.Ok)
            {
                return BadRequest(_ems.GetErrorMessage(result.Result));
            }
            return Ok(result.Auth);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(UserLoginVm userLoginVm)
        {
            var result = await _authService.LoginAsync(userLoginVm.Username, userLoginVm.Password);
            if (result.Result != UserValidationResult.Ok)
            {
                return BadRequest(_ems.GetErrorMessage(result.Result));
            }
            return Ok(result.Auth);
        }
    }
}
