using System;
using DatabaseModel.Entities.Seller;

namespace DatabaseModel.Entities.Currency
{
    public enum ValueType
    {
        Percent,
        ConstantValue
    }

    public class BillModifierEntity : Entity
    {
        public ValueType Type { get; set; }
        public decimal Value { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class UserBillModifierEntity : BillModifierEntity
    {
        public UserEntity User { get; set; }
    }

    public class SellerBillModifierEntity : BillModifierEntity
    {
        public SellerEntity Seller { get; set; }
        public ProductEntity Product { get; set; }
        public Guid ProductId { get; set; }
    }
}
