using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestaoVendasAPI.Data;
using GestaoVendasAPI.Models;

namespace GestaoVendasAPI.Controllers
{
    // Rota padrão: Quando alguém acessar "https://localhost:porta/api/produtos", cai aqui.
    [Route("api/[controller]")]
    [ApiController] // Diz ao ASP.NET que isso é uma API (ativa validações automáticas)
    public class ProdutosController : Controller
    {
        // Variável privada que vai guardar a conexão com o banco de dados
        private readonly AppDbContext _context;

        // Construtor: Aqui a Injeção de Dependência entrega o banco pronto
        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }
        //rota: api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProdutos() //Define que o método é assíncrono e que o pacote de entrega (ActionResult) conterá uma lista estruturada na memória (IEnumerable) do tipo Produto
        {
            var produtos = await _context.Produtos.ToListAsync(); //pea todos os registros e devolve numa lista
            return Ok(produtos); //O garçom entrega a bandeja para o cliente com o status HTTP 200 (OK) e a lista completa formatada em JSON.
        }
        //rota: ap/produtos/n
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id); //procura a linha exata onde a Chave Primária é igual ao número que veio na URL

            if (produto == null)
            {
                return NotFound("Produto não encontrado na base de dados.");
            }

            return Ok(produto);
        }

        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto) //Diz à API para pegar o JSON que o usuário digitou no Swagger (ou no aplicativo front-end) e converter automaticamente para os moldes da classe Produto
        {
            _context.Produtos.Add(produto); //Avisa ao Entity Framework que existe um produto novo na memória que precisa ser monitorado para inserção.
            await _context.SaveChangesAsync();//O momento da verdade. Pega tudo o que está sendo monitorado como "novo" e dispara o comando físico de INSERT INTO lá no MySQL. O ID é gerado automaticamente pelo banco neste exato milissegundo.

            return CreatedAtAction(nameof(GetProduto), new { id = produto.Id }, produto);//O padrão de excelência para cadastros. Retorna o status HTTP 201 (Criado). Além disso, ele monta a URL de onde aquele produto pode ser encontrado no futuro (apontando para o método GetProduto e passando o ID novo que o banco acabou de gerar), e finaliza devolvendo os dados do produto salvo na tela.
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id) //Se o usuário tentar atualizar o produto na URL /1, mas mandar um JSON dizendo que o ID do produto é 2, a API bloqueia a operação e devolve um Erro 400
            {
                return BadRequest("O ID da URL não corresponde ao ID do produto no corpo da requisição.");
            }

            _context.Entry(produto).State = EntityState.Modified; //Olha, este objeto já existe no banco de dados, mas ele sofreu alterações. Por favor, marque ele para ser atualizado."

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Produtos.Any(e => e.Id == id)) //Dentro do erro de concorrência, a API confere: "Esse produto ainda existe no banco?". O comando .Any() verifica rapidamente se há algum registro com aquele ID. Se não existir mais, ele avisa o usuário com um Erro 404 (Not Found). Se o erro for outro, ele joga a exceção para frente (throw;).
                {
                    return NotFound("Produto não encontrado para atualização.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent(); //Se tudo der certo e o banco atualizar o dado, a API retorna o status 204 (No Content). No padrão REST, isso significa: "Sucesso! A alteração foi feita, e eu não tenho nenhuma informação adicional para te devolver na tela".
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduto(int id)
        {
            var produto = await _context.Produtos.FindAsync(id);

            if (produto == null)
            {
                return NotFound("Produto não encontrado para exclusão.");
            }

            _context.Produtos.Remove(produto); //Aqui o garçom avisa a cozinha: "Aquele produto que acabamos de encontrar na memória? Marque ele para ser destruído".
            await _context.SaveChangesAsync(); //O Entity Framework gera o comando DELETE FROM Produtos WHERE Id = ... e dispara contra o MySQL, removendo a linha da tabela definitivamente.

            return NoContent(); //Retorna o status HTTP 204 (No Content). No padrão REST, isso significa: "A exclusão foi um sucesso absoluto, e como o item não existe mais, eu não tenho nenhum dado para te devolver na tela".
        }
    }
    
}
