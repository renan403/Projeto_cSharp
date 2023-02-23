using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;
using MVC.Services.Funcoes;
using ProdutoService = MVC.Services.ProdutoService;

namespace MVC.Controllers
{
    public class ContaController : Controller
    {
        private readonly IWebHostEnvironment _env;
        private IConfiguration _iconfig;
        public ContaController(IWebHostEnvironment env, IConfiguration iconfig)
        {
            this._env = env;
            _iconfig = iconfig;
        }
        [HttpGet]
        public IActionResult SuaConta()
        {
            if (HttpContext.Session.GetString("IdUsuario") == "" || HttpContext.Session.GetString("IdUsuario") == null)
                return RedirectToAction("HomeSair", "Home");
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        public async Task<IActionResult> Endereco()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            EnderecoService endServ = new(_iconfig);
            return View(await endServ.PuxarEnderecos(HttpContext.Session.GetString("IdUsuario")));

        }
        [HttpGet("/SuaConta/AlterarEndereco/{Key}")]
        public async Task<IActionResult> AlterarEndereco(string Key)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            var opcao = Key.Split("&");
            if (opcao[1] == "Alterar")
            {
                TempData["KeyOpcaoEnd"] = opcao[0];
            }
            else if (opcao[1] == "Padrao")
            {
                EnderecoService endServ = new(_iconfig);
                await endServ.MudarPadrao(HttpContext.Session.GetString("IdUsuario"), opcao[0]);
                return RedirectToAction("Endereco", "Conta");
            }
            else
            {
                //Retornar Endereço Padrao atualizado
                EnderecoService endServ = new(_iconfig);
                await endServ.DeletarEndereco(HttpContext.Session.GetString("IdUsuario"), $"{opcao[0] ?? "erro"}");
                var end = await endServ.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario"));
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

                EnderecoService endServ = new(_iconfig);
                await endServ.AlterarEndereco(HttpContext.Session.GetString("IdUsuario") ?? "", Key, model);
                //Retornar Endereço Padrao atualizado

                var end = await endServ.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario"));
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
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddEndereco(ModelEndereco model)
        {

            //Retornar Endereço Padrao atualizado
            EnderecoService endServ = new(_iconfig);

            await endServ.SalvarEndereco(HttpContext.Session.GetString("IdUsuario"), model);
            var end = await endServ.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario"));
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

        public IActionResult AcessoSeg()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
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
            UserService user = new(_iconfig);
            switch (botao)
            {

                case "nome":

                    await user.TrocarNome(HttpContext.Session.GetString("IdUsuario"), nome);
                    HttpContext.Session.SetString("SessaoNome", nome);
                    using (GeralService end = new())
                    {
                        HttpContext.Session.SetString("nomeFormatado", end.FormatarNomeNav(HttpContext.Session.GetString("SessaoNome") ?? ""));
                    }
                    break;
                case "excluir":
                    await user.DeletarConta(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"));
                    return RedirectToAction("HomeSair", "Home");
            }
            return RedirectToAction("AcessoSeg", "Conta");
        }
        public async Task<IActionResult> Carteira(string erro)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            ModelCartao model = new()
            {
                Erro = erro
            };

            using CartaoService cartaoServ = new(_iconfig);
            ViewBag.ArrayCartoes = await cartaoServ.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
            ViewBag.CartaoPadrao = await cartaoServ.RetornarCartaoPadrao(HttpContext.Session.GetString("IdUsuario"));

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
                        using (CartaoService cardServ = new(_iconfig))
                        {
                            await cardServ.DeleteCartao(HttpContext.Session.GetString("IdUsuario"), separador[1]);
                        }
                        break;
                    case "alterar":
                        using (CartaoService cardServ = new(_iconfig))
                        {
                            await cardServ.AlterarCartao(HttpContext.Session.GetString("IdUsuario"), separador[1], model.NomeCard, model.DataExpiracao);
                        }
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
                    using CartaoService cd = new(_iconfig);
                    var resp = await cd.SalvarCartao(HttpContext.Session.GetString("IdUsuario"), model);

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
            using (CartaoService cardServ = new(_iconfig))
            {
                await cardServ.MudarPadraoCard(HttpContext.Session.GetString("IdUsuario"), card);
            }
            return RedirectToAction("Carteira", "Conta");
        }
        public async Task<IActionResult> FinalizarCompra(string cardTrocar, string finalizar)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            var model = new ModelProduto();



            List<ModelProduto> prod = new();
            CarrinhoService carrinho = new(_iconfig);
            using CartaoService cartaoServ = new(_iconfig);
            var Produtos = await carrinho.RetornaCarrinho(HttpContext.Session.GetString("IdUsuario"));
            ContaService contaService = new(_iconfig);
            var final = await contaService.FinalizarCompra(Produtos);
            ViewBag.Carrinho = final.ProdutosEqtd;
            model.ValorTotal = final.ValorTotal;
            model.Qtd = final.QuantiaTotal;

            if (cardTrocar == null)
            {
                ViewBag.Cartao = await cartaoServ.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao"));
            }
            else
            {
                ViewBag.Cartao = await cartaoServ.Cartao(HttpContext.Session.GetString("IdUsuario"), cardTrocar);
            }

            ViewBag.ArrayCartoes = await cartaoServ.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
            if (!string.IsNullOrEmpty(finalizar))
            {
                EnderecoService endServ = new(_iconfig);
                NotaFiscalService notaServ = new(_iconfig);
                var notaFiscal = new ModelNotaFiscal
                {
                    Cartao = ViewBag.Cartao,
                    Endereco = await endServ.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario")),
                    Produto = final.Prod
                };


                await notaServ.GerarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), notaFiscal);
                for (int i = 0; i < prod.Count; i++)
                {
                    ProdutoService prodServ = new(_iconfig);

                    var produtoRt = await prodServ.RetornaProdutoPorID(prod[i].IdProduto);
                    prod[i].Qtd = produtoRt.Qtd - prod[i].Qtd;
                    prod[i].Data = null;
                    prod[i].Cancelado = null;
                    prod[i].ValorTotal = null;
                    await prodServ.AlterarProduto(HttpContext.Session.GetString("IdUsuario"), null, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), prod[i]);
                }
                CarrinhoService car = new(_iconfig);
                await car.DeletarCarrinho(HttpContext.Session.GetString("IdUsuario"));
                return RedirectToAction("FinalizarPedido", "Conta", new { endereco = ViewBag.RNCUC });
            }

            return View(model);
        }
        [HttpGet("Conta/FinalizarCompra/{Excluir}")]
        public async Task<IActionResult> ExcluirCarrinho(string excluir, int quantia)
        {
            CarrinhoService car = new(_iconfig);
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            if (excluir.Contains("***"))
            {
                var splitExcluir = excluir.Split("***");

                var idprod = splitExcluir[0].Substring(7);
                var quant = splitExcluir[1];

                var r = await car.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));

                return RedirectToAction("FinalizarCompra", "Conta");
            }

            await car.DeleteProdCarrinho(HttpContext.Session.GetString("IdUsuario"), excluir);

            return RedirectToAction("FinalizarCompra", "conta");
        }
        public IActionResult FinalizarPedido(List<string> endereco)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            ModelEndereco model = new()
            {
                Endereco = endereco[0],
                Numero = Convert.ToInt32(endereco[1]),
                Cidade = endereco[2],
                UF = endereco[3],
                Cep = endereco[4],
                Nome = endereco[5]
            };
            return View(model);
        }
        public async Task<IActionResult> Transacoes()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            using (NotaFiscalService notaServ = new(_iconfig))
            {
                ViewBag.Transacoes = await notaServ.RetornarNotaFiscal(HttpContext.Session.GetString("IdUsuario"));
            }
            return View();
        }
        public async Task<IActionResult> SeusPedidos()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            using (NotaFiscalService notaServ = new(_iconfig))
            {
                ViewBag.NotasFiscais = await notaServ.RetornarNotaFiscal(HttpContext.Session.GetString("IdUsuario"));
            }
            return View();
        }
        public async Task<IActionResult> ConfirmarSeusPedidos(string pedido, string nome)
        {
            using (NotaFiscalService notaServ = new(_iconfig))
            {
                await notaServ.ConfirmarPedido(HttpContext.Session.GetString("IdUsuario"), pedido, nome);
            }
            return RedirectToAction("SeusPedidos", "Conta");
        }
        public async Task<IActionResult> DetalhePedido(string nota)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");

            }

            CartaoService cartaoServ = new(_iconfig);
            NotaFiscalService notaServ = new(_iconfig);
            ViewBag.Cartao = await cartaoServ.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao"));
            ViewBag.NotasFiscais = await notaServ.PegarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), nota);
            var notafisc = await notaServ.PegarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), nota);
            return View(notafisc);

        }
        public async Task<IActionResult> ExibirRecibo(string nota)
        {
            ModelNotaFiscal? notafisc;
            NotaFiscalService notaServ = new(_iconfig);
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            notafisc = await notaServ.PegarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), nota);
            notafisc.IdNota = nota;

            return View(notafisc);
        }
        public IActionResult VendaNoApp()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> VendaNoApp(ModelProduto p)
        {

            var idUser = HttpContext.Session.GetString("IdUsuario") ?? "";
            if (idUser != "")
            {
                ProdutoService prod = new(_iconfig);
                var retorno = await prod.VendaNoApp(idUser, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), p);
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
                ProdutoService prod = new(_iconfig);
                var retorno = await prod.AlterarProduto(idUser, Img, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), p);
                return RedirectToAction("SeusProdutos", "Conta");
            }
            return RedirectToAction("Home", "Home");
        }
        public IActionResult AlterarProduto()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            return View();
        }
        public async Task<IActionResult> SeusProdutos()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }

            ProdutoService prod = new(_iconfig);
            ViewBag.Produtos = await prod.RetornarArrayProdutosVendedor(HttpContext.Session.GetString("IdUsuario"));

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SeusProdutos(string botao)
        {
            string[] separador = botao.Split("&&");
            string opcao = separador[0];
            string ChaveProd = separador[1];
            string foto = ProdutoService.PegarNomeUrl(separador[2]);

            switch (opcao)
            {
                case "Deletar":
                    ProdutoService prod = new(_iconfig);
                    await prod.DeletarProdAsync(ChaveProd);
                    return RedirectToAction("SeusProdutos", "Conta");
                case "Alterar":
                    return RedirectToAction("AlterarProduto", "Conta", new { Img = foto, Id = separador[3], Path = separador[4] });
            }
            return View();
        }

    }
}