using System;

namespace Exchange.Data.Entities.JoinEntities
{
    public class ProductProductClassEntity
    {
        public Guid ProductId { get; set; }
        public string ProductClassId { get; set; }
        public ProductEntity Product { get; set; }
        public ProductClassEntity ProductClass { get; set; }
    }
}
