using System.Threading.Tasks;
using Exchange.Authentication;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;
using Exchange.Core.Services.ErrorMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Exchange.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly ErrorMessageService _ems;

        public AuthController(AuthService authService, ErrorMessageService ems)
        {
            _authService = authService;
            _ems = ems;
        }

        [HttpGet("refreshToken")]
        [AllowAnonymous]
        public async Task<IActionResult> RefreshTokenAsync(string refreshToken)
        {
            var result = await _authService.RefreshTokenAsync(refreshToken);
            if (result.Result != AuthValidationResult.Ok)
            {
                return BadRequest(_ems.GetErrorMessage(result.Result));
            }
            return Ok(result.Auth);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> LoginAsync(UserLoginDto userLoginDto)
        {
            var result = await _authService.LoginAsync(userLoginDto.Username, userLoginDto.Password);
            if (result.Result != AuthValidationResult.Ok)
            {
                return BadRequest(_ems.GetErrorMessage(result.Result));
            }
            return Ok(result.Auth);
        }
    }
}
