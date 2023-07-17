using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Repository;
using System.Diagnostics;
using System.Text;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;
 
namespace MVC.Controllers
{
    public class HomeController : Controller
    {   
        private readonly IConfiguration _iconfig;
        private HttpClient? client;
        private readonly IGeralService _IGeral;
        public HomeController(IConfiguration iconfig , IGeralService geralService)
        {
            _iconfig = iconfig;
            _IGeral = geralService; 
        }
        [HttpGet]
        public async Task<IActionResult> Home()
        {
            try
            {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
                client = new()
                {
                    BaseAddress = new Uri(_iconfig.GetValue<string>("UrlApi"))
                };
                HttpResponseMessage response = await client.GetAsync("Product/GetAll");
                if (response.IsSuccessStatusCode)
                { 
                    ViewBag.Produtos = await response.Content.ReadFromJsonAsync<Dictionary<string, ModelProduto>>(); // se houver erro, o motivo é que está nulo e com erro
                }                            
            }
            catch (Exception)
            {
                ViewBag.Produtos = new Dictionary<string, ModelProduto>();// caindo na exception inciado o produto para não dar erro no FOR da pagina
            }
            return View();
        }        
        [HttpGet("/Home/Home/Sair")]
        public IActionResult HomeSair()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}