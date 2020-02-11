using System;
using System.Collections.Generic;
using DatabaseModel.Entities.JoinEntities;

namespace DatabaseModel.Entities
{
    public class ProductEntity: Entity
    {
        public string Name { get; set; }
        public Guid? UserId { get; set; }
        public UserEntity? Owner { get; set; }
        public virtual ICollection<ProductProductClassEntity>? ProductProductClasses { get; set; }
        public virtual ICollection<ProductClassAttributeValueEntity>? ProductClassAttributeValues { get; set; }
    }
}
