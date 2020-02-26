using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.Models.Dto
{
    public class UserRegistrationDto
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] [EmailAddress] public string Email { get; set; }
    }
}
