using System.Collections.Generic;
using DatabaseModel.Constants;

namespace DatabaseModel.Entities
{
    public class UserEntity : Entity
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<ProductEntity>? Products { get; set; }
        public virtual ICollection<ValueChangeRequestEntity>? ChangeRequests { get; set; }
        public virtual ICollection<UserDeviceLoginEntity>? UserDeviceLogins { get; set; }
    }
}
