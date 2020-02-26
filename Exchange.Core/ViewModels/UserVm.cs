using System;
using System.ComponentModel.DataAnnotations;
using Exchange.Data.Constants;
using Exchange.Data.Entities.User;

namespace Exchange.Core.ViewModels
{
    public class UserVm
    {
        public UserVm()
        {
        }

        public UserVm(UserEntity user)
        {
            Username = user.Username;
            Email = user.Email;
            Role = user.Role;
            Id = user.Id;
            IsEmailConfirmed = user.IsEmailConfirmed;
        }
        [Required]
        public Guid Id { get; set; }
        [Required]
        public string Username { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }

        public bool IsEmailConfirmed { get; set; }
    }
}
