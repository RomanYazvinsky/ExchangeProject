using System;

namespace Exchange.Entities.JoinEntities
{
    public class ProductProductClass
    {
        public Guid ProductId { get; set; }
        public string ProductClassId { get; set; }
        public Product Product { get; set; }
        public ProductClass ProductClass { get; set; }
    }
}