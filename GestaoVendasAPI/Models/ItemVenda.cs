using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasAPI.Models
{
    public class ItemVenda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int VendaId { get; set; } // FK apontando para a Venda

        //'VendaId' se refere à classe Venda
        [ForeignKey("VendaId")]
        public Venda? Venda { get; set; }

        [Required]
        public int ProdutoId { get; set; } // FK apontando para o Produto

        [ForeignKey("ProdutoId")]
        public Produto? Produto { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser pelo menos 1.")]
        public int Quantidade { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecoUnitario { get; set; } // Preço do produto no instante da venda
    }
}
