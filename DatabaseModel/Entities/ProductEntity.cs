using System;
using System.Collections.Generic;
using Exchange.Data.Entities.Currency;
using Exchange.Data.Entities.JoinEntities;
using Exchange.Data.Entities.Order;
using Exchange.Data.Entities.Seller;

namespace Exchange.Data.Entities
{
    public class ProductEntity: Entity
    {
        public string Name { get; set; }
        public Guid? SellerId { get; set; }
        public SellerEntity? Seller { get; set; }
        public virtual ICollection<ProductProductClassEntity>? ProductProductClasses { get; set; }
        public virtual ICollection<ProductClassAttributeValueEntity>? ProductClassAttributeValues { get; set; }
        public virtual ICollection<OrderEntity>? Orders { get; set; }
        public virtual SellerBillModifierEntity? Discount { get; set; }
        public virtual Guid? DiscountId { get; set; }
    }
}
