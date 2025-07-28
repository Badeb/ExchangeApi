using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExchangeApi.Models.Entities
{
    public class FavoriteQueries
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(3)]
        public required string Base {  get; set; }
        [Required]
        [StringLength(3)]
        public required string Target { get; set; }

        [Required]
        [StringLength(50)]
        public required string Name { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,6)")]
        public decimal Rate { get; set; }


    }
}
