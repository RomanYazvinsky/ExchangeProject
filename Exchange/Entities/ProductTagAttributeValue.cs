using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange.Entities
{
    public class ProductTagAttributeValue: Entity
    {
        public long ProductId { get; set; }
        public long ProductTagAttributeId { get; set; }
        [ForeignKey(nameof(ProductId))]
        public Product Product { get; set; }
        [ForeignKey(nameof(ProductTagAttributeId))]
        public ProductTagAttribute ProductTagAttribute { get; set; }
        public string Value { get; set; }
    }
}