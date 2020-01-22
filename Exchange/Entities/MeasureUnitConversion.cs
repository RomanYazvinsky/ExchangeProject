using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exchange.Entities
{
    public class MeasureUnitConversion
    {
        [Key, Column(Order = 0)] public long FromId { get; set; }
        [Key, Column(Order = 1)] public long ToId { get; set; }
        [ForeignKey(nameof(FromId))] public MeasureUnit From { get; set; }
        [ForeignKey(nameof(ToId))] public MeasureUnit To { get; set; }
        public decimal Coefficient { get; set; }
    }
}