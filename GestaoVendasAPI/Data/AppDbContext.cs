// Traz o Entity Framework Core para podermos usar a classe DbContext
using Microsoft.EntityFrameworkCore;
// Traz os nossos modelos para que o DbContext saiba o que transformar em tabela
using GestaoVendasAPI.Models;

namespace GestaoVendasAPI.Data
{
    public class AppDbContext : DbContext
    {
        // Construtor que recebe as configurações do banco (como a string de conexão) e repassa para a classe base.
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Os DbSets representam as tabelas que serão criadas no banco de dados MySQL.
        // O nome que você dá aqui (ex: "Produtos") será o nome exato da tabela lá no banco.
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Venda> Vendas { get; set; }
        public DbSet<ItemVenda> ItensVenda { get; set; }
    }
}