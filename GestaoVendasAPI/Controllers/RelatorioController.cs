using GestaoVendasAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using System.Text;
// Lembre-se de importar a pasta dos seus Models e do seu Contexto aqui

namespace GestaoVendasAPI.Controllers
{
    public class RelatorioController : Controller
    {
        private readonly AppDbContext _context; 

        public RelatorioController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Busca as vendas grampeando o Cliente e os Itens
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Itens!)
                    .ThenInclude(i => i.Produto)
                .ToListAsync();

            // Entrega a lista de vendas para a tela HTML desenhar
            return View(vendas);
        }

        public async Task<IActionResult> GerarPdf()
        {
            // 1. Buscamos os dados reais do banco
            var vendas = await _context.Vendas
                .Include(v => v.Cliente)
                .Include(v => v.Itens!)
                    .ThenInclude(i => i.Produto)
                .ToListAsync();

            // 2. Desenhamos o HTML da página do PDF direto na memória (com Bootstrap)
            var sb = new StringBuilder();
            sb.Append(@"
            <!DOCTYPE html>
            <html lang='pt-br'>
            <head>
                <link href='https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css' rel='stylesheet'>
                <style> body { -webkit-print-color-adjust: exact; } </style>
            </head>
            <body class='p-5'>
                <div class='text-center mb-4'>
                    <h1 class='fw-bold text-primary'>Relatório Oficial de Vendas</h1>
                    <p class='text-muted'>Gerado automaticamente pelo sistema</p>
                </div>
                <table class='table table-bordered table-striped'>
                    <thead class='table-dark'>
                        <tr>
                            <th>ID da Venda</th>
                            <th>Cliente</th>
                            <th>Data</th>
                            <th>Valor Total</th>
                        </tr>
                    </thead>
                    <tbody>");

            // 3. Injetamos as linhas da tabela dinamicamente
            foreach (var v in vendas)
            {
                sb.Append($@"
                        <tr>
                            <td class='fw-bold'>{v.Id}</td>
                            <td>{v.Cliente?.Nome}</td>
                            <td>{v.DataVenda:dd/MM/yyyy HH:mm}</td>
                            <td class='text-success fw-bold'>{v.ValorTotal:C}</td>
                        </tr>");
            }

            sb.Append(@"
                    </tbody>
                </table>
            </body>
            </html>");

            // 4. Acionamos o Navegador Fantasma (Puppeteer)
            // Ele baixa o motor do Chromium na primeira vez que rodar
            var browserFetcher = new BrowserFetcher();
            await browserFetcher.DownloadAsync();

            using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            using var page = await browser.NewPageAsync();

            // Entregamos o nosso HTML para o navegador carregar
            await page.SetContentAsync(sb.ToString());

            // Tiramos a "foto" em formato PDF A4
            var pdfBytes = await page.PdfDataAsync(new PdfOptions
            {
                Format = PaperFormat.A4,
                PrintBackground = true
            });

            // 5. Devolvemos o arquivo pronto para o usuário baixar
            return File(pdfBytes, "application/pdf", "RelatorioVendas.pdf");
        }
    }
}