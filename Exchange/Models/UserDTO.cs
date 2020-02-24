using System;
using DatabaseModel.Constants;
using DatabaseModel.Entities;

namespace Exchange.Models
{
    public class UserDTO
    {
        public UserDTO()
        {
        }

        public UserDTO(UserEntity user)
        {
            Username = user.UserName;
            Email = user.Email;
            Role = user.Role;
            Id = user.Guid;
            IsEmailConfirmed = user.IsEmailConfirmed;
        }

        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }
}
