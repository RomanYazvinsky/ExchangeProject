using Exchange.Data.Entities.User;

namespace Exchange.Core.Models.Dto
{
    public class EmailConfirmationDto
    {
        public EmailConfirmationDto()
        {
        }
        public EmailConfirmationDto(UserEntity user)
        {
            this.UserId = user.Id.ToString();
            this.Email = user.Email;
            this.Username = user.Username;
        }
        public EmailConfirmationDto(UserDto user)
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
