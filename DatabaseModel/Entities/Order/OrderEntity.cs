using System;
using System.Collections.Generic;
using DatabaseModel.Entities.Currency;
using DatabaseModel.Entities.JoinEntities;

namespace DatabaseModel.Entities.Order
{
    public enum OrderStatus
    {
        Created,
        OnReview,
        Payed,
        InWork,
        Discussion,
        Rejected,
        RejectedByUser,
        Complete
    }

    public class OrderEntity: Entity
    {
        public Guid CustomerId { get; set; }
        public UserEntity Customer { get; set; }
        public Guid ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal Quantity { get; set; }

        public decimal Bill { get; set; }

        public OrderStatus Status { get; set; }

        public AddressEntity Address { get; set; }
        public Guid AddressId { get; set; }
        public ICollection<OrderTransactionEntity>? Transactions { get; set; }
        public ICollection<OrderToBillModifier> Modifiers { get; set; }
    }
}
