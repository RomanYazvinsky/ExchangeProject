using System;
using Exchange.Data.Entities.Currency;
using Exchange.Data.Entities.Order;

namespace Exchange.Data.Entities.JoinEntities
{
    public class OrderToBillModifier
    {
        public OrderEntity Order { get; set; }
        public Guid OrderId { get; set; }
        public BillModifierEntity BillModifier { get; set; }
        public Guid BillModifierId { get; set; }
    }
}
