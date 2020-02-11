using System;

namespace DatabaseModel.Entities
{
    public class UserDeviceLoginEntity: Entity
    {
        public Guid UserId { get; set; }
        public UserEntity User { get; set; }
        public DateTime ValidUntil { get; set; }
        public string DeviceInfo { get; set; }
    }
}
