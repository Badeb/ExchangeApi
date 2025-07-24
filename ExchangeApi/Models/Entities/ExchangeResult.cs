using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeApi.Models.Entities
{
    public class ExchangeResult
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Column(TypeName = "decimal(18,6)")]
        public decimal Rate { get; set; }
        public DateTime TimeStamp { get; set; }

        [ForeignKey(nameof(CurrencyQuery))]
        public int CurrencyQueryId { get; set; }


    }
}
