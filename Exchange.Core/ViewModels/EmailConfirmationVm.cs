using Exchange.Data.Entities.User;

namespace Exchange.Core.ViewModels
{
    public class EmailConfirmationVm
    {
        public EmailConfirmationVm()
        {
        }
        public EmailConfirmationVm(UserEntity user)
        {
            this.UserId = user.Id.ToString();
            this.Email = user.Email;
            this.Username = user.Username;
        }
        public EmailConfirmationVm(UserVm user)
        {
            this.UserId = user.Id.ToString();
            this.Email = user.Email;
            this.Username = user.Username;
        }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
