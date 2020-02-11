using System;
using DatabaseModel.Entities.Seller;

namespace DatabaseModel.Entities
{
    public enum RequestStatus
    {
        Waiting,
        Submitted,
        Rejected,
    }

    public class ValueChangeRequestEntity : Entity
    {
        public Guid ProductClassAttributeValueId { get; set; }
        public ProductClassAttributeValueEntity AttributeValue { get; set; }

        public Guid? SenderId { get; set; }
        public SellerEntity? Sender { get; set; }
        public DateTime ResolutionTime { get; set; }
        public DateTime CreationTime { get; set; }
        public RequestStatus Status { get; set; }
        public string Value { get; set; }
        public string PreviousValue { get; set; }
    }
}
