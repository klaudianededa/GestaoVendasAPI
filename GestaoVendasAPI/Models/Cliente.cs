using System.ComponentModel.DataAnnotations;

namespace GestaoVendasAPI.Models
{
    public class Cliente
    {
        [Key] //chave primária
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        [StringLength(150)] //maximo 150 caracteres
        public string Nome { get; set; } = string.Empty; //empty pra iniciar vazio, e não nulo

        [Required(ErrorMessage = "O e-mail é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de e-mail inválido.")] // Valida se tem "@" e ".com"
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string Telefone { get; set; } = string.Empty;

        // Ao criar um novo cliente, a data de cadastro já recebe o dia e hora atuais automaticamente
        public DateTime DataCadastro { get; set; } = DateTime.Now;
    }
}
