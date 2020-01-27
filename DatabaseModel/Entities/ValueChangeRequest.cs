using System;

namespace Exchange.Entities
{
    public enum RequestStatus
    {
        Waiting,
        Submitted,
        Rejected,
    }

    public class ValueChangeRequest : Entity
    {
        public Guid ProductClassAttributeValueId { get; set; }
        public ProductClassAttributeValue AttributeValue { get; set; }

        public Guid? SenderId { get; set; }
        public User? Sender { get; set; }
        public DateTime ResolutionTime { get; set; }
        public DateTime CreationTime { get; set; }
        public RequestStatus Status { get; set; }
        public string Value { get; set; }
        public string PreviousValue { get; set; }
    }
}