using System;

namespace Exchange.Models
{
    public class AuthDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserDTO UserInfo { get; set; }
        public DateTime ServerUtcNow { get; set; }
        public DateTime AccessUtcValidTo { get; set; }
    }
}
