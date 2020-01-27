using System;
using System.Collections.Generic;

namespace Exchange.Entities
{
    public class ProductClassAttribute: Entity
    {
        public string Name { get; set; }
        public string AssociatedClassId { get; set; }
        public ProductClass AssociatedClass { get; set; }
        public ValueDataType ValueDataType { get; set; }
        public bool Mandatory { get; set; }
        public virtual ICollection<ProductClassAttributeValue>? AttributeValues { get; set; }
    }

    public enum ValueDataType
    {
        Text,
        RichText,
        Number,
        Decimal,
        DateTime,
        VideoUrl,
        AudioUrl,
        ImageUrl,
        Url,
        LinkToProduct
    }
}
