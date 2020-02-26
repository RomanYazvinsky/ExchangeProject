using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.ViewModels
{
    public class UserLoginVm
    {
        [Required] public string Username { get; set; }

        [Required] public string Password { get; set; }
    }
}
