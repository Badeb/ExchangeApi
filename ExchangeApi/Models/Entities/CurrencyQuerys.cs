using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeApi.Models.Entities
{
    public class CurrencyQuery
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(3)]
        public string Base { get; set; }
        [Required]
        [StringLength(3)]
        public string Target { get; set; }

       public ICollection<ExchangeResult> Results { get; set; } = new List<ExchangeResult>();


    }
}
