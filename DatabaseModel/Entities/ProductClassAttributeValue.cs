using System;
using System.Collections.Generic;

namespace Exchange.Entities
{
    public class ProductClassAttributeValue: Entity
    {
        public Guid ProductId { get; set; }
        public Product Product { get; set; }
        public Guid ProductClassAttributeId { get; set; }
        public ProductClassAttribute ProductClassAttribute { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ValueChangeRequest>? ChangeRequests { get; set; }
    }
}
