using System;

namespace Exchange.Data.Entities
{
    public class MeasureUnitConversionEntity
    {
        public Guid FromId { get; set; }
        public Guid ToId { get; set; }
        public MeasureUnitEntity From { get; set; }
        public MeasureUnitEntity To { get; set; }
        public decimal Coefficient { get; set; }
    }
}
