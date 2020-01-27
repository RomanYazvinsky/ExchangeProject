using System.Collections.Generic;
using Exchange.Entities.JoinEntities;

namespace Exchange.Entities
{
    public class ProductClass
    {
        public string Name { get; set; }
        public virtual ICollection<ProductProductClass>? ProductProductClasses { get; set; }
        public virtual ICollection<ProductClassAttribute>? ProductClassAttributes { get; set; }
    }
}
