using System;

namespace Exchange.Entities
{
    public class UserDeviceLogin: Entity
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public DateTime ValidUntil { get; set; }
        public string DeviceInfo { get; set; }
    }
}