using System;
using System.Collections.Generic;
using Exchange.Entities.JoinEntities;

namespace Exchange.Entities
{
    public class Product: Entity
    {
        public string Name { get; set; }
        public Guid? UserId { get; set; }
        public User? Owner { get; set; }
        public decimal Amount { get; set; }
        public virtual ICollection<ProductProductClass>? ProductProductClasses { get; set; }
        public virtual ICollection<ProductClassAttributeValue>? ProductClassAttributeValues { get; set; }
    }
}
