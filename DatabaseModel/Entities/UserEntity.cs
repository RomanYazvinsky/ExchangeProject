using System;
using System.Collections.Generic;
using DatabaseModel.Constants;
using DatabaseModel.Entities.Currency;
using DatabaseModel.Entities.Order;
using DatabaseModel.Entities.Seller;

namespace DatabaseModel.Entities
{
    public class UserEntity : Entity
    {
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }

        public SellerEntity? Seller { get; set; }
        public Guid? SellerId { get; set; }
        public virtual ICollection<UserDeviceLoginEntity>? UserDeviceLogins { get; set; }

        public virtual ICollection<OrderEntity>? Orders { get; set; }
        public virtual ICollection<UserBillModifierEntity>? Discounts { get; set; }
    }
}
