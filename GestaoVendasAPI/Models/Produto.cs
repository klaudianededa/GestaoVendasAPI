// Traz a biblioteca do C# que contém as ferramentas para validar os dados (como [Required] e [StringLength]).
using System.ComponentModel.DataAnnotations;

// Traz a biblioteca do C# que permite dar ordens diretas de como o banco de dados deve ser criado fisicamente (como [Column]).
using System.ComponentModel.DataAnnotations.Schema;

// Define o "endereço" desta classe no projeto. Ela mora na pasta 'Models' dentro do projeto 'GestaoVendasAPI'.
namespace GestaoVendasAPI.Models
{
    // Cria a classe pública 'Produto', que funciona como o molde principal para todos os itens que a loja vai vender.
    public class Produto
    {
        // Avisa ao Entity Framework que a propriedade de baixo será a Chave Primária (PK) na tabela, gerando IDs automaticamente (1, 2, 3...).
        [Key]
        // Propriedade que guarda o número de identificação único do produto. Pode ser lida (get) e alterada (set).
        public int Id { get; set; }

        // Impede que o produto seja salvo sem nome. Se tentarem, a API devolve a mensagem de erro escrita aqui.
        [Required(ErrorMessage = "O nome do produto é obrigatório.")]
        // Limita o tamanho do texto no banco para 100 letras. O {1} é substituído automaticamente pelo número '100' na hora de mostrar o erro.
        [StringLength(100, ErrorMessage = "O nome deve ter no máximo {1} caracteres.")]
        // Propriedade que guarda o nome do produto. O '= string.Empty' garante que ela comece vazia, nunca como 'nula', evitando que o sistema quebre.
        public string Nome { get; set; } = string.Empty;

        // Limita a descrição para no máximo 250 caracteres no banco de dados. (Como não tem o [Required] em cima, é opcional preencher).
        [StringLength(250, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
        // Propriedade que guarda os detalhes do produto, também iniciando em branco por segurança.
        public string Descricao { get; set; } = string.Empty;

        // Impede que o produto seja salvo no banco sem um preço definido.
        [Required(ErrorMessage = "O preço é obrigatório.")]
        // Força o banco de dados (SQL Server ou MySQL) a criar essa coluna no formato Decimal, suportando 18 dígitos no total, sendo 2 após a vírgula (ex: 1500,50).
        [Column(TypeName = "decimal(18,2)")]
        // Cria uma regra de negócio: o preço mínimo aceito é 1 centavo, e o máximo é 999.999,99. Impede que cadastrem itens de graça ou com preço negativo.
        [Range(0.01, 999999.99, ErrorMessage = "O preço deve ser maior que zero.")]
        // Propriedade que guarda o valor financeiro. O tipo 'decimal' é obrigatório no C# para lidar com dinheiro sem perder precisão nos centavos.
        public decimal Preco { get; set; }

        // Impede que o cadastro seja feito sem informar quanto de estoque o produto tem no momento.
        [Required(ErrorMessage = "A quantidade em estoque é obrigatória.")]
        // Regra de negócio: o estoque pode ser 0 (vazio), mas nunca negativo (como -5). O 'int.MaxValue' é o limite máximo de números que o C# suporta.
        [Range(0, int.MaxValue, ErrorMessage = "A quantidade não pode ser negativa.")]
        // Propriedade que guarda quantas unidades existem fisicamente na loja (número inteiro, pois não dá para vender "meio" teclado).
        public int QuantidadeEstoque { get; set; }

        // Impede que o produto fique sem um grupo (ex: Informática, Roupas, Acessórios).
        [Required(ErrorMessage = "A categoria é obrigatória.")]
        // Limita o nome da categoria a 50 caracteres no banco de dados.
        [StringLength(50, ErrorMessage = "A categoria deve ter no máximo {1} caracteres.")]
        // Propriedade que guarda a categoria do produto, iniciando em branco para prevenir erros.
        public string Categoria { get; set; } = string.Empty;

    }
}