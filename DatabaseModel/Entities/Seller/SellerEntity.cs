﻿using System;
using System.Collections.Generic;
using DatabaseModel.Entities.Currency;

namespace DatabaseModel.Entities.Seller
{
    public class SellerEntity: Entity
    {
        public string SellerName { get; set; }
        public AddressEntity Address { get; set; }
        public Guid AddressId { get; set; }

        public virtual ICollection<ProductEntity>? Products { get; set; }
        public virtual ICollection<ValueChangeRequestEntity>? ChangeRequests { get; set; }
        public virtual ICollection<UserEntity> Users { get; set; }

        public virtual ICollection<SellerBillModifierEntity>? Discounts { get; set; }

    }
}
