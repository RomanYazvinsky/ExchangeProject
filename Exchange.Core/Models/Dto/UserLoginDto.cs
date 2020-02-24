using System.ComponentModel.DataAnnotations;
using Exchange.Core.Constants;

namespace Exchange.Web.Models.Dto
{
    public class UserLoginDto
    {
        [Required]
        [StringLength(CredentialsValidationConstants.MaximumUsernameLength,
            MinimumLength = CredentialsValidationConstants.MinimalUsernameLength)]
        public string Username { get; set; }

        [Required]
        [StringLength(CredentialsValidationConstants.MaximumPasswordLength,
            MinimumLength = CredentialsValidationConstants.MinimalPasswordLength)]
        public string Password { get; set; }
    }
}
