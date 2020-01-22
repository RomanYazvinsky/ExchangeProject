using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange.Entities
{
    public class Product
    {
        public string Name { get; set; }
        public long UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public User Owner { get; set; }
        public long Amount { get; set; }
        public virtual ICollection<ProductTag> ProductTags { get; set; }
        public virtual ICollection<ProductTagAttributeValue> ProductTagAttributeValues { get; set; }
    }
}