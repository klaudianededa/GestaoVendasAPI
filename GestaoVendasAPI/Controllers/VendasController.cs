using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasAPI.Data;
using GestaoVendasAPI.Models;

namespace GestaoVendasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VendasController(AppDbContext context)
  
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Venda>>> GetVendas()
        {
            var vendas = await _context.Vendas
                .Include(v => v.Cliente!)
                .Include(v => v.Itens!)
                    .ThenInclude(i => i.Produto)
                .ToListAsync();

            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Venda>> GetVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Cliente!)
                .Include(v => v.Itens!)
                    .ThenInclude(i => i.Produto)
                .FirstOrDefaultAsync(v => v.Id ==id);

            if (venda == null)
            {
                return NotFound("Venda não encontrada.");
            }

            return Ok(venda);
        }

        [HttpPost]
        public async Task<ActionResult<Venda>> PostVenda(Venda venda)
        {
            var cliente = await _context.Clientes.FindAsync(venda.ClienteId);

            //verificar se o cliente existe 
            if (cliente == null)
            {
                return NotFound("Não é possível realizar a venda: Cliente não encontrado.");
            }

            //validar se o carrinho não está vazio
            if (venda.Itens == null || !venda.Itens.Any())
            {
                return BadRequest("A venda não pode ser processada sem nenhum produto.");
            }

            //preparando a nota fiscal
            venda.ValorTotal = 0;
            venda.DataVenda = DateTime.Now;

            //como se estivesse passando os produtos na esteira
            foreach (var item in venda.Itens)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);

                if (produto == null)
                {
                    return NotFound($"Produto com ID {item.ProdutoId} não encontrado.");
                }

                if (produto.QuantidadeEstoque < item.Quantidade)
                {
                    return BadRequest($"Estoque insuficiente para o produto: {produto.Nome}. Estoque atual: {produto.QuantidadeEstoque}");
                }

                // Dar baixa no estoque físico e somar o preço real do banco
                produto.QuantidadeEstoque -= item.Quantidade;
                item.PrecoUnitario = produto.Preco;
                venda.ValorTotal += (produto.Preco * item.Quantidade);
            }

            // 4. Salvar tudo (Venda, Itens e Baixa de Estoque)
            _context.Vendas.Add(venda);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVenda), new { id = venda.Id }, venda);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVenda(int id)
        {
            var venda = await _context.Vendas
                .Include(v => v.Itens!)
                .FirstOrDefaultAsync(v => v.Id == id); ;

            if (venda == null)
            {
                return NotFound("Venda não encontrada para cancelamento.");
            }

            if (venda.Itens != null)
            {
                foreach (var item in venda.Itens)
                {
                    var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                    if (produto != null)
                    {
                        produto.QuantidadeEstoque += item.Quantidade;
                    }
                }
            }

            _context.Vendas.Remove(venda);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}