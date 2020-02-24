using System.Collections.Generic;

namespace Exchange.Data.Entities
{
    public class MeasureUnitEntity : Entity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public virtual ICollection<MeasureUnitConversionEntity>? Conversions { get; set; }
    }
}
