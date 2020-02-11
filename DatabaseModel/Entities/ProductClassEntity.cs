using System.Collections.Generic;
using DatabaseModel.Entities.JoinEntities;

namespace DatabaseModel.Entities
{
    public class ProductClassEntity
    {
        public string Name { get; set; }
        public virtual ICollection<ProductProductClassEntity>? ProductProductClasses { get; set; }
        public virtual ICollection<ProductClassAttributeEntity>? ProductClassAttributes { get; set; }
    }
}
