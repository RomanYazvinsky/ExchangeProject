using System;
using DatabaseModel.Entities.Currency;
using DatabaseModel.Entities.Order;

namespace DatabaseModel.Entities.JoinEntities
{
    public class OrderToBillModifier
    {
        public OrderEntity Order { get; set; }
        public Guid OrderId { get; set; }
        public BillModifierEntity BillModifier { get; set; }
        public Guid BillModifierId { get; set; }
    }
}
