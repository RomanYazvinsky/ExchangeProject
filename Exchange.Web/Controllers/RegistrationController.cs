using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto;
using Exchange.Core.Models.Dto.Validation;
using Exchange.Core.Services;
using Exchange.Core.Services.ErrorMessages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Exchange.Web.Controllers
{
    [ApiController]
    [Route("api")]
    public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly UserRegistrationService _userRegistrationService;
        private readonly CredentialValidationService _credentialValidationService;
        private readonly ErrorMessageService _ems;

        public RegistrationController(
            ILogger<RegistrationController> logger,
            UserRegistrationService userRegistrationService,
            CredentialValidationService credentialValidationService,
            ErrorMessageService ems
        )
        {
            _logger = logger;
            _userRegistrationService = userRegistrationService;
            _credentialValidationService = credentialValidationService;
            _ems = ems;
        }

        [HttpPost("validateUsername")]
        [AllowAnonymous]
        public async Task<Dictionary<string, string>?> ValidateUsernameAsync(UsernameDto username)
        {
            var errors = await _credentialValidationService.ValidateUsernameAsync(username.Username);
            return errors?.ToDictionary(error => error.ToString(), error => _ems.GetErrorMessage(error));
        }

        [HttpPost("validatePassword")]
        [AllowAnonymous]
        public Dictionary<string, string>? ValidatePassword(PasswordDto password)
        {
            var errors = _credentialValidationService.ValidatePassword(password.Password);
            return errors?.ToDictionary(error => error.ToString(), error => _ems.GetErrorMessage(error));
        }

        [HttpPost("validateEmail")]
        [AllowAnonymous]
        public async Task<Dictionary<string, string>?> ValidateEmailAsync(EmailDto email)
        {
            var errors = await _credentialValidationService.ValidateEmailAsync(email.Email);
            return errors?.ToDictionary(error => error.ToString(), error => _ems.GetErrorMessage(error));
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterAsync(UserRegistrationDto registrationDto)
        {
            var user = await _userRegistrationService.RegisterUser(
                registrationDto.Username,
                registrationDto.Password,
                registrationDto.Email);
            var mailError = await _userRegistrationService.SendConfirmationEmail(user);
            if (mailError != MailConfirmationResult.Ok && mailError != MailConfirmationResult.AlreadyConfirmed)
            {
                return BadRequest(_ems.GetErrorMessage(mailError));
            }

            return Ok();
        }

        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(EmailConfirmationDto confirmation)
        {
            var error = await _userRegistrationService.ConfirmEmail(confirmation.UserId);
            if (error != MailConfirmationResult.Ok && error != MailConfirmationResult.AlreadyConfirmed)
            {
                return BadRequest(_ems.GetErrorMessage(error));
            }

            return Ok();
        }
    }
}
