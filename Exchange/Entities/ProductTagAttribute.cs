using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange.Entities
{
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
        BlobUrl
    }
    
    public class ProductTagAttribute: Entity
    {
        public string Name { get; set; }
        public long AssociatedTagId { get; set; }
        [ForeignKey(nameof(AssociatedTagId))]
        public ProductTag AssociatedTag { get; set; }
        public ValueDataType ValueDataType { get; set; }
        public bool Mandatory { get; set; }
    }
}