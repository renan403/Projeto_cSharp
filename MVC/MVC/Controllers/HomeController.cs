using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Cryptography;
using MVC.Services;
using MVC.Services.Funcoes;

namespace MVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _iconfig;
        private HttpClient? client;
        public HomeController(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }
        [HttpGet]
        public async Task<IActionResult> Home()
        {

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
                using (GeralService geral = new())
                {
                    ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                    ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
                }
            }
            catch (Exception)
            {
                ViewBag.Produtos = new Dictionary<string, ModelProduto>();
            }
            return View();
        }
        [HttpGet]
        public IActionResult teste()
        {
            return View();
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