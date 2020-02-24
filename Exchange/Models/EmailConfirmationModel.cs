using DatabaseModel.Entities;

namespace Exchange.Models
{
    public class EmailConfirmationModel
    {
        public EmailConfirmationModel()
        {
        }
        public EmailConfirmationModel(UserEntity user)
        {
            this.UserId = user.Guid.ToString();
            this.Email = user.Email;
            this.Username = user.UserName;
        }
        public string UserId { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
    }
}
