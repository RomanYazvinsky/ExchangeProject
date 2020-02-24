using System;
using System.Collections.Generic;
using Exchange.Data.Constants;
using Exchange.Data.Entities.Currency;
using Exchange.Data.Entities.Order;
using Exchange.Data.Entities.Seller;

namespace Exchange.Data.Entities.User
{
    public class UserEntity : Entity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Email { get; set; }
        public Role Role { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public SellerEntity? Seller { get; set; }
        public Guid? SellerId { get; set; }

        public virtual ICollection<OrderEntity>? Orders { get; set; }
        public virtual ICollection<UserBillModifierEntity>? Discounts { get; set; }
    }
}
