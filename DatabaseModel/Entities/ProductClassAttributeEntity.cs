using System.Collections.Generic;

namespace Exchange.Data.Entities
{
    public class ProductClassAttributeEntity: Entity
    {
        public string Name { get; set; }
        public string AssociatedClassId { get; set; }
        public ProductClassEntity AssociatedClass { get; set; }
        public ValueDataType ValueDataType { get; set; }
        public bool Mandatory { get; set; }
        public virtual ICollection<ProductClassAttributeValueEntity>? AttributeValues { get; set; }
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
