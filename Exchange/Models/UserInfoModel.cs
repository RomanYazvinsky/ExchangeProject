using Exchange.Entities;

namespace Exchange.Models
{
    public class UserInfoModel
    {
        public UserInfoModel()
        {
        }

        public UserInfoModel(User user)
        {
            Username = user.UserName;
            Email = user.Email;
            Role = user.Role;
        }

        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
    }
}
