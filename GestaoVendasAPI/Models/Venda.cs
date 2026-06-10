using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestaoVendasAPI.Models
{
    public class Venda
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ClienteId { get; set; } // Chave Estrangeira (FK) no banco

        //ClienteId aponta para a classe Cliente
        [ForeignKey("ClienteId")]
        public Cliente? Cliente { get; set; }

        public DateTime DataVenda { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")] //18 caracteres, 2 após a vírgula
        public decimal ValorTotal { get; set; }
        public ICollection<ItemVenda>? Itens { get; set; }
    }
}
