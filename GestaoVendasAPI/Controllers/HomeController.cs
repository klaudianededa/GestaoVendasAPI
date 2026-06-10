using Microsoft.AspNetCore.Mvc;

namespace GestaoVendasAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()//A rota padrão (/) aciona o método Index().
        {
            return View(); //O comando return View(); manda o servidor buscar um arquivo HTML correspondente a essa ação.
        }
    }
}