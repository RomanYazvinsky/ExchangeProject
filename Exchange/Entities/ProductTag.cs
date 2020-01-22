using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange.Entities
{
    public class ProductTag
    {
        [Key, Column(Order = 0)] public string Name { get; set; }
        [Key, Column(Order = 1)] public long ProductId { get; set; }
        [ForeignKey(nameof(ProductId))] public Product Product { get; set; }
    }
}