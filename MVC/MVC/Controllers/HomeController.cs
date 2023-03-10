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
            // return RedirectToAction("teste");


            try
            {
                client = new()
                {
                    BaseAddress = new Uri(_iconfig.GetValue<string>("UrlApi"))
                };
                HttpResponseMessage response = await client.GetAsync("Product/GetAll");
                if (response.IsSuccessStatusCode)
                {

                    ViewBag.Produtos = await response.Content.ReadFromJsonAsync<Dictionary<string, ModelProduto>>();
                }
              
                    ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                    ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
                
            }
            catch (Exception)
            {
                ViewBag.Produtos = new Dictionary<string, ModelProduto>();
            }
            return View();
        }
        [HttpGet]
        public PartialViewResult PartialTest(TestModel test)
        {

            return PartialView(test);
        }
        [HttpPost]
        public PartialViewResult PartialTest2(TestModel test)
        {

            return PartialView(test);
        }
        [HttpGet]
        public IActionResult teste()
        {
            var m = new TestModel() { Info = "1" };
            return View(m);
        }     
        [HttpGet("/Home/Home/Sair")]
        public IActionResult HomeSair()
        {
            HttpContext.Session.SetString("SessaoNome", "");
            HttpContext.Session.SetString("Endereço", "");
            HttpContext.Session.SetString("nomeFormatado", "");
            HttpContext.Session.SetString("IdUsuario", "");
            HttpContext.Session.SetString("Senha", "");
            return RedirectToAction("Home");
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}