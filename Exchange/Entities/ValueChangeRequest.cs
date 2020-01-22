using System;
using System.ComponentModel.DataAnnotations.Schema;

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
        public long EntityId { get; set; }
        public long? SenderId { get; set; }
        [ForeignKey(nameof(SenderId))] public User Sender { get; set; }
        public DateTime ResolutionTime { get; set; }
        public DateTime CreationTime { get; set; }
        public RequestStatus Status { get; set; } = RequestStatus.Waiting;
        public string Value { get; set; }
    }

    public class AttributeValueChangeRequest : ValueChangeRequest
    {
        [ForeignKey(nameof(EntityId))] public ProductTagAttributeValue AttributeValue { get; set; }
    }
}