using System.Collections.Generic;
using Exchange.Data.Entities.JoinEntities;

namespace Exchange.Data.Entities
{
    public class ProductClassEntity
    {
        public string Name { get; set; }
        public virtual ICollection<ProductProductClassEntity>? ProductProductClasses { get; set; }
        public virtual ICollection<ProductClassAttributeEntity>? ProductClassAttributes { get; set; }
    }
}
