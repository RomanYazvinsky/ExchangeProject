using System;
using System.Collections.Generic;

namespace DatabaseModel.Entities
{
    public class ProductClassAttributeValueEntity: Entity
    {
        public Guid ProductId { get; set; }
        public ProductEntity Product { get; set; }
        public Guid ProductClassAttributeId { get; set; }
        public ProductClassAttributeEntity ProductClassAttribute { get; set; }
        public string Value { get; set; }

        public virtual ICollection<ValueChangeRequestEntity>? ChangeRequests { get; set; }
    }
}
