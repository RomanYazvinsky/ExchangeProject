using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.ViewModels
{
    public class UserRegistrationVm
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
    }
}
