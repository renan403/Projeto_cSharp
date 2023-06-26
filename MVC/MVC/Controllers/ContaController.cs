using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MVC.Funcoes;
using MVC.Models;
using MVC.Repository;
using MVC.Services;
using ProdutoService = MVC.Services.ProdutoService;

namespace MVC.Controllers
{
    public class ContaController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration _iconfig;
        private readonly IUserService _iUser;
        private readonly IGeralService _IGeral;
        private readonly ICartaoService _cartao;
        private readonly IEnderecoService _endereco;
        private readonly ICarrinhoService _carrinho;
        private readonly INotaFiscalService _notaFiscal;
        private readonly IProdutoService _produto;

        public ContaController(IWebHostEnvironment env, IConfiguration iconfig, IUserService userService,IGeralService geralService, ICartaoService cartao, IEnderecoService endereco, ICarrinhoService carrinho, INotaFiscalService notaFiscal, IProdutoService produto)
        {
            this._env = env;
            _iconfig = iconfig;
            _iUser = userService;
            _IGeral = geralService;
            _cartao = cartao;
            _endereco = endereco;
            _carrinho = carrinho;
            _notaFiscal = notaFiscal;
            _produto = produto;
        }
        [HttpGet]
        public IActionResult SuaConta()
        {
            if (HttpContext.Session.GetString("IdUsuario") == "" || HttpContext.Session.GetString("IdUsuario") == null)
                return RedirectToAction("HomeSair", "Home");

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            return View();
        }
        public async Task<IActionResult> Endereco()
        { 
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            return View(await _endereco.PuxarEnderecos(HttpContext.Session.GetString("IdUsuario")));

        }
        [HttpGet("/SuaConta/AlterarEndereco/{Key}")]
        public async Task<IActionResult> AlterarEndereco(string Key)
        {

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            

            var opcao = Key.Split("&");
            if (opcao[1] == "Alterar")
            {
                TempData["KeyOpcaoEnd"] = opcao[0];
            }
            else if (opcao[1] == "Padrao")
            {
                await _endereco.MudarPadrao(HttpContext.Session.GetString("IdUsuario"), opcao[0]);
                return RedirectToAction("Endereco", "Conta");
            }
            else
            {
                //Retornar Endereço Padrao atualizado
                await _endereco.DeletarEndereco(HttpContext.Session.GetString("IdUsuario"), $"{opcao[0] ?? "erro"}");
                var end = await _endereco.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario"));
                if (end != null)
                {
                    HttpContext.Session.SetString("Endereço", $"{end.Endereco}/{end.Numero}/{end.Cidade}/{end.UF}/{end.Cep}/{end.Nome}");
                }
                else
                {
                    HttpContext.Session.SetString("Endereço", $"Sem endereço/");
                }
                return RedirectToAction("Endereco", "Conta");
            }

            return View();
        }
        [HttpPost("/SuaConta/AlterarEndereco/{Key}")]
        public async Task<IActionResult> AlterarEndereco(ModelEndereco model)
        {
            var Key = TempData["KeyOpcaoEnd"].ToString();
            if (Key != string.Empty)
            {

                await _endereco.AlterarEndereco(HttpContext.Session.GetString("IdUsuario") ?? "", Key, model);
                //Retornar Endereço Padrao atualizado

                var end = await _endereco.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario"));
                if (end == null)
                {
                    HttpContext.Session.SetString("Endereço", $"Cadastre");
                }
                else
                {
                    HttpContext.Session.SetString("Endereço", $"{end.Endereco}/{end.Numero}/{end.Cidade}/{end.UF}/{end.Cep}/{end.Nome}");
                }
                return RedirectToAction("Endereco", "Conta");
            }
            return RedirectToAction("Endereco", "Conta");
        }
        [HttpGet]
        public IActionResult AddEndereco()
        {

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEndereco(ModelEndereco model)
        {

            //Retornar Endereço Padrao atualizado

            await _endereco.SalvarEndereco(HttpContext.Session.GetString("IdUsuario"), model);
            var end = await _endereco.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario"));
            if (end == null)
            {
                HttpContext.Session.SetString("Endereço", $"Cadastre");
            }
            else
            {

                HttpContext.Session.SetString("Endereço", $"{end.Endereco}/{end.Numero}/{end.Cidade}/{end.UF}/{end.Cep}/{end.Nome}");
            }
            return RedirectToAction("Endereco", "Conta");


        }
        [HttpPost]
        public async void AlterarQuantidadeCarrinho(string id, string quantidade)
        {
            await _carrinho.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), id, Convert.ToInt32(quantidade));
        }

        public IActionResult AcessoSeg()
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            var model = new ModelLogin
            {
                Email = HttpContext.Session.GetString("SessaoEmail"),
                Nome = HttpContext.Session.GetString("SessaoNome")
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> AcessoSeg(string botao, string nome, string email)
        {
            
            switch (botao)
            {
                case "nome":

                    await _iUser.TrocarNome(HttpContext.Session.GetString("IdUsuario"), nome);
                    HttpContext.Session.SetString("SessaoNome", nome);
         
                        HttpContext.Session.SetString("nomeFormatado", _IGeral.FormatarNomeNav(HttpContext.Session.GetString("SessaoNome") ?? ""));
                    
                    break;
                case "excluir":
                    await _iUser.DeletarConta(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"));
                    return RedirectToAction("HomeSair", "Home");
            }
            return RedirectToAction("AcessoSeg", "Conta");
        }
        public async Task<IActionResult> Carteira(string erro)
        {

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            ModelCartao model = new()
            {
                Erro = erro
            };

         
            ViewBag.ArrayCartoes = await _cartao.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
            ViewBag.CartaoPadrao = await _cartao.RetornarCartaoPadrao(HttpContext.Session.GetString("IdUsuario"));

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Carteira(ModelCartao model, string btn)
        {
            if (!string.IsNullOrEmpty(btn))
            {
                var separador = btn.Split("%%");
                btn = separador[0];
                switch (btn)
                {
                    case "remove":
                       
                            await _cartao.DeleteCartao(HttpContext.Session.GetString("IdUsuario"), separador[1]);
                        
                        break;
                    case "alterar":
                       
                            await _cartao.AlterarCartao(HttpContext.Session.GetString("IdUsuario"), separador[1], model.NomeCard, model.DataExpiracao);
                        
                        break;
                }
            }


            if (!string.IsNullOrEmpty(model.Bandeira) || !string.IsNullOrEmpty(model.NumeroCard))
            {
                using Card card = new();
                card.Bandeira = model.Bandeira ?? "";
                card.Cartao = model.NumeroCard ?? "";
                var valido = card.CartaoValido();
                if (valido)
                {
                    switch (model.Bandeira)
                    {
                        case "DinerClub":
                            model.CaminhoImgBandeira = "../img/Cartoes/bandeira/dinerclub.png";
                            model.CaminhoImgCartao = "../img/Cartoes/DinerClub.png";
                            break;
                        case "MasterCard":
                            model.CaminhoImgBandeira = "../img/Cartoes/bandeira/mastercard.png";
                            model.CaminhoImgCartao = "../img/Cartoes/mastercardBlack.png";
                            break;
                        case "Visa":
                            model.CaminhoImgBandeira = "../img/Cartoes/bandeira/Visa.png";
                            model.CaminhoImgCartao = "../img/Cartoes/Visa.png";
                            break;
                        case "Amex":
                            model.CaminhoImgBandeira = "../img/Cartoes/bandeira/Amex.png";
                            model.CaminhoImgCartao = "../img/Cartoes/AMEX_Preto.jpg";
                            break;
                    }
                    
                    var resp = await _cartao.SalvarCartao(HttpContext.Session.GetString("IdUsuario"), model);

                    if (resp.Contains("sucesso"))
                        return RedirectToAction("Carteira", "Conta");
                    else
                        return RedirectToAction("Carteira", "Conta", new { erro = resp });
                }
                return RedirectToAction("Carteira", "Conta", new { erro = "Cartão inválido ⚠️" });
            }

            return RedirectToAction("Carteira", "Conta");
        }
        public async Task<IActionResult> AlterarCartao(string card)
        {
            
                await _cartao.MudarPadraoCard(HttpContext.Session.GetString("IdUsuario"), card);
            
            return RedirectToAction("Carteira", "Conta");
        }
        public async Task<IActionResult> FinalizarCompra(string cardTrocar, string finalizar)
        {
 
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            var model = new ModelProduto();



            //List<ModelProduto> prod = new();
         
            var Produtos = await _carrinho.RetornaCarrinho(HttpContext.Session.GetString("IdUsuario"));
         
            var final = await _produto.FinalizarCompra(Produtos);
            ViewBag.Carrinho = final.ProdutosEqtd;
            model.ValorTotal = Convert.ToDouble(((decimal)final.ValorTotal).ToString("N2"));
            model.Qtd = final.QuantiaTotal;

            if (cardTrocar == null)
            {
                ViewBag.Cartao = await _cartao.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao"));
            }
            else
            {
                ViewBag.Cartao = await _cartao.Cartao(HttpContext.Session.GetString("IdUsuario"), cardTrocar);
            }

            ViewBag.ArrayCartoes = await _cartao.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
            //if (!string.IsNullOrEmpty(finalizar))
            //{
            //    var notaFiscal = new ModelNotaFiscal
            //    {
            //        Cartao = ViewBag.Cartao,
            //        Endereco = await _endereco.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario")),
            //        Produto = final.Prod
            //    };


            //    await _notaFiscal.GerarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), notaFiscal);
            //    for (int i = 0; i < prod.Count; i++)
            //    {

            //        var produtoRt = await _produto.RetornaProdutoPorID(prod[i].IdProduto);
            //        prod[i].Qtd = produtoRt.Qtd - prod[i].Qtd;
            //        prod[i].Data = null;
            //        prod[i].Cancelado = null;
            //        prod[i].ValorTotal = null;
            //        await _produto.AlterarProduto(HttpContext.Session.GetString("IdUsuario"), null, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), prod[i]);
            //    }
            //    await _carrinho.DeletarCarrinho(HttpContext.Session.GetString("IdUsuario"));
            //    return RedirectToAction("FinalizarPedido", "Conta", new { endereco = ViewBag.RNCUC });
            //}

            return View(model);
        }
        public async Task<ActionResult> redirectFinalVenda()
        {
            var Produtos = await _carrinho.RetornaCarrinho(HttpContext.Session.GetString("IdUsuario"));
            var final = await _produto.FinalizarCompra(Produtos);
            final.ValorTotal = Convert.ToDouble(((decimal)final.ValorTotal).ToString("N2"));
            foreach(var i in final.Prod)
            {
               // i.ValorTotalStr = ((decimal)final.ValorTotal).ToString("N2");
                i.ValorTotalStrPorProd = ((decimal)(i.QtdPorProd * i.PrecoProd)).ToString("N2");
            }
            var notaFiscal = new ModelNotaFiscal
            {
                Cartao = await _cartao.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao")),
                Endereco = await _endereco.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario")),
                Produto = final.Prod
            };
            await _notaFiscal.GerarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), notaFiscal);

            foreach(var i in Produtos)
            {
                var produtoRt = await _produto.RetornaProdutoPorID(i.Value.IdProduto);
                i.Value.Qtd = produtoRt.Qtd - i.Value.QtdPorProd;
                i.Value.Cancelado = null; 
                await _produto.AlterarProduto(HttpContext.Session.GetString("IdUsuario"), null, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), i.Value);
            }
            

           

            await _carrinho.DeletarCarrinho(HttpContext.Session.GetString("IdUsuario"));
            ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            return RedirectToAction("FinalizarPedido", "Conta", new { endereco = ViewBag.RNCUC });

        }
        [HttpGet("Conta/FinalizarCompra/{Excluir}")]
        public async Task<IActionResult> ExcluirCarrinho(string excluir, int quantia)
        {
  
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            if (excluir.Contains("***"))
            {
                var splitExcluir = excluir.Split("***");

                var idprod = splitExcluir[0].Substring(7);
                var quant = splitExcluir[1];

                var r = await _carrinho.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));

                return RedirectToAction("FinalizarCompra", "Conta");
            }

            await _carrinho.DeleteProdCarrinho(HttpContext.Session.GetString("IdUsuario"), excluir);

            return RedirectToAction("FinalizarCompra", "conta");
        }
        public IActionResult FinalizarPedido(List<string> endereco)
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            ModelEndereco model = new()
            {
                Endereco = ViewBag.RNCUC[0],
                Numero = Convert.ToInt32(ViewBag.RNCUC[1]),
                Cidade = ViewBag.RNCUC[2],
                UF = ViewBag.RNCUC[3],
                Cep = ViewBag.RNCUC[4],
                Nome = ViewBag.RNCUC[5]
            };
            return View(model);
        }
        public async Task<IActionResult> Transacoes()
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            
                ViewBag.Transacoes = await _notaFiscal.RetornarNotaFiscal(HttpContext.Session.GetString("IdUsuario"));
            
            return View();
        }
        public async Task<IActionResult> SeusPedidos()
        {
          
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            
                ViewBag.NotasFiscais = await _notaFiscal.RetornarNotaFiscal(HttpContext.Session.GetString("IdUsuario"));
            
            return View();
        }
        public async Task<IActionResult> ConfirmarSeusPedidos(string pedido, string nome)
        {
                await _notaFiscal.ConfirmarPedido(HttpContext.Session.GetString("IdUsuario"), pedido, nome);
            return RedirectToAction("SeusPedidos", "Conta");
        }
        public async Task<IActionResult> DetalhePedido(string nota)
        {
           
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");

            

            
            ViewBag.Cartao = await _cartao.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao"));
            ViewBag.NotasFiscais = await _notaFiscal.PegarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), nota);
            var notafisc = await _notaFiscal.PegarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), nota);
            return View(notafisc);

        }
        public async Task<IActionResult> ExibirRecibo(string nota)
        {
            ModelNotaFiscal? notafisc;
           
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            

            notafisc = await _notaFiscal.PegarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), nota);
            notafisc.IdNota = nota;

            return View(notafisc);
        }
        public IActionResult VendaNoApp()
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VendaNoApp(ModelProduto p)
        {
            
            var idUser = HttpContext.Session.GetString("IdUsuario") ?? "";
            if (idUser != "")
            {
                
                var retorno = await _produto.VendaNoApp(idUser, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), p);
                return RedirectToAction("SuaConta", "Conta");
            }

            return RedirectToAction("Home", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AlterarProduto(ModelProduto p, string Img, string id, string path)
        {
            var idUser = HttpContext.Session.GetString("IdUsuario") ?? "";
            if (idUser != "")
            {
                p.IdProduto = id;
                var retorno = await _produto.AlterarProduto(idUser, Img, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), p);
                return RedirectToAction("SeusProdutos", "Conta");
            }
            return RedirectToAction("Home", "Home");
        }
        public IActionResult AlterarProduto()
        {
          
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            return View();
        }
        public async Task<IActionResult> SeusProdutos()
        {

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            

            ViewBag.Produtos = await _produto.RetornarArrayProdutosVendedor(HttpContext.Session.GetString("IdUsuario"));

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SeusProdutos(string botao)
        {
            string[] separador = botao.Split("&&");
            string opcao = separador[0];
            string ChaveProd = separador[1];
            string foto = _produto.PegarNomeUrl(separador[2]);

            switch (opcao)
            {
                case "Deletar":
                    await _produto.DeletarProdAsync(ChaveProd);
                    return RedirectToAction("SeusProdutos", "Conta");
                case "Alterar":
                    return RedirectToAction("AlterarProduto", "Conta", new { Img = foto, Id = separador[3], Path = separador[4] });
            }
            return View();
        }

    }
}