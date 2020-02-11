using System;
using System.Collections.Generic;
using DatabaseModel.Entities.Currency;
using DatabaseModel.Entities.JoinEntities;
using DatabaseModel.Entities.Order;
using DatabaseModel.Entities.Seller;

namespace DatabaseModel.Entities
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
