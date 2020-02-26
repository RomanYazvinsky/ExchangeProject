using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.Models.Dto
{
    public class UserLoginDto
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}
