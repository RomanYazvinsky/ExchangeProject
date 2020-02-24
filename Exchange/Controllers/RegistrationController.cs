using System.Threading.Tasks;
using Exchange.Constants;
using Exchange.Models;
using Exchange.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace Exchange.Controllers
{
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly ILogger<RegistrationController> _logger;
        private readonly UserRegistrationService _userRegistrationService;
        private readonly ErrorMessageService _ems;

        public RegistrationController(
            ILogger<RegistrationController> logger,
            UserRegistrationService userRegistrationService,
            ErrorMessageService ems
        )
        {
            _logger = logger;
            _userRegistrationService = userRegistrationService;
            _ems = ems;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register(UserRegistrationModel registrationModel)
        {
            var error = await _userRegistrationService.ValidateUsername(registrationModel.Username);
            if (error != null)
            {
                return BadRequest(_ems.GetErrorMessage(error.Value));
            }

            var passwordError = _userRegistrationService.ValidatePassword(registrationModel.Password);
            if (passwordError != null)
            {
                return BadRequest(_ems.GetErrorMessage(passwordError.Value));
            }

            var emailError = _userRegistrationService.ValidateEmail(registrationModel.Email);
            if (emailError != null)
            {
                return BadRequest(_ems.GetErrorMessage(emailError.Value));
            }

            var origin = Request.Headers[HeaderNames.Origin];
            var confirmationUrl = registrationModel.EmailConfirmationUrl;
            if (string.IsNullOrWhiteSpace(origin) || string.IsNullOrWhiteSpace(confirmationUrl))
            {
                return BadRequest(_ems.GetErrorMessage(MailConfirmationErrorTypes.InvalidConfirmationUrl));
            }
            var username = registrationModel.Username;
            var password = registrationModel.Username;
            var email = registrationModel.Email;
            var user = await _userRegistrationService.RegisterUser(username, password, email);

            var mailError = await _userRegistrationService.SendConfirmationEmail($"{origin}/{confirmationUrl}",user);
            if (mailError != null)
            {
                return Problem(statusCode: StatusCodes.Status503ServiceUnavailable,
                    detail: _ems.GetErrorMessage(mailError.Value));
            }
            return Ok();
        }

        [HttpPost("confirmEmail")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(EmailConfirmationModel confirmation)
        {
            var error = await _userRegistrationService.ConfirmEmail(confirmation.ConfirmationId);
            if (error != null)
            {
                return BadRequest(_ems.GetErrorMessage(error.Value));
            }
            return Ok();
        }
    }
}
