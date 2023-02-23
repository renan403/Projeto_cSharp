using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using System.Diagnostics;
using Newtonsoft.Json;
using Microsoft.Extensions.Primitives;
using MVC.Services;
using MVC.Services.Funcoes;
using Firebase.Auth;
using System.Drawing;

namespace MVC.Controllers
{
    public class LoginController : Controller
    {
        private readonly IConfiguration _iconfig;
        public LoginController(IConfiguration config)
        {
            _iconfig = config;
        }

        [HttpGet]
        public IActionResult Login(string? Tela)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            var user = new ModelLogin();
            TempData["Tela"] = Tela;

            return View(user);
        }
        [HttpPost]
        public async Task<ActionResult>  PartialLogin(ModelLogin user)
        {
            string nome;
            string? idUsuario;

            using (UserService serv = new(_iconfig))
            {
                var login = await serv.Logar(user.Email ?? "", user.Senha ?? "");

                if (login == "Logged")
                {
                    using EnderecoService endServ = new(_iconfig);
                    idUsuario = await serv.RetornaID(user.Email ?? "");
                    nome = await serv.RetornaNome(idUsuario ?? "");

                    var end = await endServ.RetornarEndPadrao(idUsuario);

                    if (end.Endereco == null && end.Numero == 0)
                        HttpContext.Session.SetString("Endereço", $"Cadastre");
                    else
                        HttpContext.Session.SetString("Endereço", $"{end.Endereco}/{end.Numero}/{end.Cidade}/{end.UF}/{end.Cep}/{end.Nome}");

                    using CartaoService cartaoServ = new(_iconfig);
                    var cartoes = await cartaoServ.ReturnCard(idUsuario);
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
                        using GeralService endGeral = new();
                        HttpContext.Session.SetString("nomeFormatado", endGeral.FormatarNomeNav(HttpContext.Session.GetString("SessaoNome") ?? ""));
                    }
                    user.Exist = 1;
                }else if(login == "TOO_MANY_ATTEMPTS_TRY_LATER")
                {
                    ViewBag.Erro = "Acesso desabilitado";
                    ViewBag.MessageErro = "Muitas falhas de login, para entrar imediatamente você pode trocar a senha ou pode tentar novamente mais tarde.";
                    ViewBag.SrcErro = "../icones/exclamation-triangle.svg";
                }else if(login == "EMAIL_NOT_FOUND")
                {
                    ViewBag.Erro = "Email";
                    ViewBag.MessageErro = "Email não encontrado.";
                    ViewBag.SrcErro = "../icones/exclamation-triangle.svg";
                }else if(login == "INVALID_PASSWORD")
                {
                    ViewBag.Erro = "Senha";
                    ViewBag.MessageErro = "Senha não incorreta.";
                    ViewBag.SrcErro = "../icones/exclamation-triangle.svg";
                }

                if (string.IsNullOrEmpty(login)) 
                {
                    ViewBag.Erro = "Sistema";
                    ViewBag.MessageErro = "Sistema está fora do ar, por favor tente mais tarde";
                    ViewBag.SrcErro = "../icones/1873373.svg";
                }                   
            }
            
            return PartialView("PartialLogin", user);
        }


       

        [HttpGet]
        public IActionResult CriarConta()
        {
            ModelUsuario model = new();
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View(model);
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

                using UserService serv = new(_iconfig);
                 var rt = await serv.RegistrarUser(model.Nome, model.Email, model.Senha);
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
            ModelLogin model = new();
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> RecuperarSenha(ModelLogin model)
        {

            TempData["email"] = model.Email;
            if (TempData["email"] as string != "" || TempData["email"] != null)
            {
                UserService user = new(_iconfig);               
                var resp = await user.ResetarSenha(model.Email ?? "");
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
