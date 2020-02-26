using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.Models.Dto.Validation
{
    public class UsernameDto
    {
        [Required]
        public string Username { get; set; }
    }
}
