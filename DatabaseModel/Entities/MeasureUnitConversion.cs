using System;

namespace Exchange.Entities
{
    public class MeasureUnitConversion
    {
        public Guid FromId { get; set; }
        public Guid ToId { get; set; }
        public MeasureUnit From { get; set; }
        public MeasureUnit To { get; set; }
        public decimal Coefficient { get; set; }
    }
}