using System;
using DatabaseModel.Entities.Currency;

namespace DatabaseModel.Entities.Order
{
    public enum OrderTransactionType
    {
        OrderPayment,
        Refund,
    }
    public enum OrderTransactionStatus
    {
        InWork,
        Complete,
        RejectedByPaymentSystem
    }

    public class OrderTransactionEntity: Entity
    {
        public OrderEntity Order { get; set; }
        public Guid OrderId { get; set; }
        public OrderTransactionType Type { get; set; }
        public decimal Value { get; set; }
        public CurrencyEntity Currency { get; set; }
        public Guid CurrencyId { get; set; }
        public OrderTransactionStatus Status { get; set; }
        public DateTime TransactionStart { get; set; }
        public DateTime? TransactionEnd { get; set; }
        public string? Description { get; set; }
    }
}
