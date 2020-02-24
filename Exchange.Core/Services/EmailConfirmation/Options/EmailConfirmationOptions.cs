using System.ComponentModel.DataAnnotations;

namespace Exchange.Core.Services.EmailConfirmation.Options
{
    public class EmailConfirmationOptions
    {
        public string SmtpServer { get; set; }
        public int Port { get; set; }
        public string Account { get; set; }
        public string? Password { get; set; }
        public bool? UseSsl { get; set; }
        public bool UseOAuth { get; set; }
        public string? AuthToken { get; set; }
        [Required]
        public string ConfirmationUrl { get; set; }
        [Required] public string EmailTemplatePath { get; set; }
    }
}
