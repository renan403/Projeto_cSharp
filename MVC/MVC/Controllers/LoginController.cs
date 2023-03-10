using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Repository;
using System.Diagnostics;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _iconfig;
        private readonly IUserService _iUser;
        private readonly IGeralService _IGeral;
        private readonly ICartaoService _cartao;
        private readonly IEnderecoService _endereco;

        public LoginController(IConfiguration config, IUserService iuser,IGeralService geralService, ICartaoService cartao, IEnderecoService endereco)
        {
            _IGeral = geralService;
            _iUser = iuser;
            _iconfig = config;
            _cartao = cartao;
            _endereco = endereco;
        }

        [HttpGet]
        public IActionResult Login(string? Tela)
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");

            TempData["Tela"] = Tela;

            return View(new ModelLogin());
        }
        [HttpPost]
        public async Task<ActionResult>  PartialLogin(ModelLogin user)
        {
    
            var retornoUser = await _iUser.Logar(user.Email, user.Senha); 
            if(retornoUser.Resposta == "Logged")
            {
                string? idUsuario = await _iUser.RetornaID(user.Email);
                string? nome = await _iUser.RetornaNome(idUsuario);              
               var end = await _endereco.RetornarEndPadrao(idUsuario);
               
               if (end.Endereco == null && end.Numero == 0)
                   HttpContext.Session.SetString("Endereço", $"Cadastre");
               else
                   HttpContext.Session.SetString("Endereço", $"{end.Endereco}/{end.Numero}/{end.Cidade}/{end.UF}/{end.Cep}/{end.Nome}");
               
               
               var cartoes = await _cartao.ReturnCard(idUsuario);
               foreach (var cartao in cartoes)
               {
                   HttpContext.Session.SetString("cartao", cartao.Key);
                   break;
               }
               
               HttpContext.Session.SetString("IdUsuario", idUsuario);
               HttpContext.Session.SetString("Senha", user.Senha ?? "");
               HttpContext.Session.SetString("SessaoEmail", user.Email ?? "");
               HttpContext.Session.SetString("SessaoNome", nome);
               
               if (!string.IsNullOrEmpty(HttpContext.Session.GetString("SessaoNome")))// Verificar sessão para não dar erro nas condições
               {
                   HttpContext.Session.SetString("nomeFormatado", _IGeral.FormatarNomeNav(HttpContext.Session.GetString("SessaoNome") ?? ""));
               }
               user.Exist = 1;

            }
            else
            {
                ViewBag.Erro = retornoUser.Erro;
                ViewBag.MessageErro = retornoUser.MessageErro;
                ViewBag.SrcErro = retornoUser.SrcErro;
            }      
            
            return PartialView("PartialLogin", user);
        }


       

        [HttpGet]
        public IActionResult CriarConta()
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            return View(new ModelUsuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CriarConta(ModelUsuario model)
        {
            if (model.ConfSenha != model.Senha)
            {
                ModelState.AddModelError("ConfSenha", "Senhas estão diferentes");
            }
            if (ModelState.IsValid)
            {

               
                 var rt = await _iUser.RegistrarUser(model.Nome, model.Email, model.Senha);
                if (rt == "Criado")
                {
                    TempData["email"] = model.Email;
                }
                else
                {
                    ModelState.AddModelError("ConfSenha", rt == null ? "🌐 Serviço indisponivel, tente mais tarde!" : rt);
                    return View(model);
                }


                return RedirectToAction("EmailEnviado", "Login");
            }
            return View(model);
        }
        public IActionResult EmailEnviado()
        {
            string email = TempData["email"] as string ?? "";
            if (email != "")
            {
                ViewData["ContaEmailEnviado"] = TempData["email"];
                return View();
            }
            return RedirectToAction("Home", "Home");
        }
        [HttpGet]
        public IActionResult RecuperarSenha()
        {
            return View(new ModelLogin());
        }
        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(ModelLogin model)
        {

            TempData["email"] = model.Email;
            if (TempData["email"] as string != "" || TempData["email"] != null)
            {
                             
                var resp = await _iUser.ResetarSenha(model.Email ?? "");
                if (resp == "Altered")
                {
                    return RedirectToAction("EmailEnviado", "Login");
                }
                model.Resposta = resp;
                return View(model);
            }
            return RedirectToAction("Home", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
