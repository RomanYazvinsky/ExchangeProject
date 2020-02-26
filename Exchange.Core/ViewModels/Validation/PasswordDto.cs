using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.Models.Dto.Validation
{
    public class PasswordDto
    {
        [Required]
        public string Password { get; set; }
    }
}
