using System;

namespace Exchange.Models
{
    public class AuthInfoModel
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public UserInfoModel UserInfo { get; set; }
        public DateTime ServerUtcNow { get; set; }
        public DateTime AccessUtcValidTo { get; set; }
    }
}
