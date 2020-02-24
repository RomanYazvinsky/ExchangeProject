using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.Models.Dto.Validation
{
    public class EmailDto
    {
        [Required]
        public string Email { get; set; }
    }
}
