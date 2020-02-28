using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Exchange.Core.Constants.Errors;
using Exchange.Core.Models.Dto.Validation;
using Exchange.Core.Services;
using Exchange.Core.ViewModels;
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
        private readonly IUserRegistrationService _userRegistrationService;
        private readonly ICredentialValidationService _credentialValidationService;
        private readonly IErrorMessageService _ems;

        public RegistrationController(
            ILogger<RegistrationController> logger,
            IUserRegistrationService userRegistrationService,
            ICredentialValidationService credentialValidationService,
            IErrorMessageService ems
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
        public async Task<IActionResult> RegisterAsync(UserRegistrationVm registrationVm)
        {
            var (user, errorMessages) = await _userRegistrationService.RegisterUser(
                registrationVm.Username,
                registrationVm.Password,
                registrationVm.Email
            );
            if (user == default)
            {
                return BadRequest(errorMessages.ToDictionary(pair => pair.Key.ToString(), pair => pair.Value));
            }
            var mailError = _userRegistrationService.SendConfirmationEmail(user);
            if (mailError != MailConfirmationResult.Ok && mailError != MailConfirmationResult.AlreadyConfirmed)
            {
                return BadRequest(_ems.GetErrorMessage(mailError));
            }

            return Ok();
        }

        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailAsync(EmailConfirmationVm confirmation)
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
