using System.Collections.Generic;

namespace Exchange.Entities
{
    public enum Role
    {
        Administrator,
        Operator,
        Customer,
        Disabled,
    }

    public class User: Entity
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<Product>? Products { get; set; }
        public virtual ICollection<ValueChangeRequest>? ChangeRequests { get; set; }
        public virtual ICollection<UserDeviceLogin>? UserDeviceLogins { get; set; }

    }
}
