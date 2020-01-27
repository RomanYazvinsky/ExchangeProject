using System.Collections.Generic;

namespace Exchange.Entities
{
    public class MeasureUnit : Entity
    {
        public string Name { get; set; }
        public string ShortName { get; set; }
        public virtual ICollection<MeasureUnitConversion>? Conversions { get; set; }
    }
}
