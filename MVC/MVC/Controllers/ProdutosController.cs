using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;
using System.Runtime.Intrinsics.Arm;
using MVC.Services.Funcoes;

namespace MVC.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IConfiguration _iconfig;
        public ProdutosController(IConfiguration iconfig)
        {
            this._iconfig = iconfig;
        }

        [HttpGet("Produtos/Produtos/{prodId}")]
        public async Task<IActionResult> Produtos(string prodId)
        {
            ProdutoService prodServ = new(_iconfig);
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            ViewBag.Logado = HttpContext.Session.GetString("IdUsuario") == null || HttpContext.Session.GetString("IdUsuario") == "" ? false : true;
            string IdProduto = prodId;
            ModelProduto? model = new();
            if (prodId.Contains("Erro***"))
            {
                var urlErro = prodId.IndexOf("***");

                IdProduto = prodId[(urlErro + 3)..];



                model = await prodServ.RetornaProdutoPorID(IdProduto);
                var quantia = prodId[0].ToString();
                model.ErroProd = $"Máximo até 5 produtos iguais no carrinho, quantidade atual {quantia}.";
                ViewBag.pagProduto = model;
                TempData["prodId"] = IdProduto;
                return View(model);

            }
            try
            {
                ViewBag.pagProduto = model = await prodServ.RetornaProdutoPorID(IdProduto);
            }
            catch (Exception)
            {

                ViewBag.pagProduto = model = new ModelProduto();
            }

            TempData["prodId"] = IdProduto;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Produtos(int select, string botao)
        {
            var idP = TempData["prodId"] as string;
            if (idP != null)
            {
                switch (botao)
                {
                    case "addCar":
                        CarrinhoService carrinho = new(_iconfig);


                        var jaPossuiNoCarrinho = await carrinho.PossuiNoCarrinho(HttpContext.Session.GetString("IdUsuario"), idP);
                        if (jaPossuiNoCarrinho)
                        {
                            string resp = await carrinho.AddQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idP, select);
                            if (resp == "sucesso")
                            {
                                return RedirectToAction("Carrinho", "Produtos");
                            }
                            return RedirectToAction("Produtos", "Produtos", new { prodId = $"{resp}Erro***{idP}" });
                        }


                        var result = await carrinho.SalvarProdCarrinho(HttpContext.Session.GetString("IdUsuario"), idP, select);


                        return RedirectToAction("Carrinho", "Produtos");

                    case "Comprar":
                        ProdutoService prodServ = new(_iconfig);
                        CompraService Serv = new(_iconfig);
                        ModelProduto produto = await prodServ.RetornaProdutoPorID(idP);
                        produto.Qtd = select;
                        produto.Cancelado = false;
                        produto.Data = DateTime.Now.ToString("dd/MM/yyyy");
                        produto.ValorTotal = (float)Math.Round(((decimal)produto.PrecoProd * select), 2);
                        await Serv.AddItemUnico(HttpContext.Session.GetString("IdUsuario"), produto);

                        return RedirectToAction("ComprarProd", "Produtos", new { produto = idP, quantia = select });
                }

            }
            return RedirectToAction("Error", "Home");

        }
        [HttpGet("Produtos/ComprarProd/{produto}")]
        public async Task<IActionResult> ComprarProd(string produto, int quantia, string cardTrocar, string finalizar)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            var model = new ModelProduto();
            List<dynamic> ProdutosEqtd = new();
            List<ModelProduto> prod = new();

            using CartaoService cartaoServ = new(_iconfig);
            List<int?> qtdProdutos = new();

            CompraService Serv = new(_iconfig);

            ModelProduto item = await Serv.PegarProduto(HttpContext.Session.GetString("IdUsuario"));
            List<dynamic> unirLista = new();

            unirLista.Add(item);
            unirLista.Add(quantia);
            unirLista.Add(item.IdProduto);

            ProdutosEqtd.Add(unirLista);
            ViewBag.Carrinho = ProdutosEqtd;
            model.ValorTotal = item.ValorTotal;
            model.Qtd = item.Qtd;


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
                prod.Add(item);
                EnderecoService endServ = new(_iconfig);
                NotaFiscalService notaServ = new(_iconfig);
                var notafiscal = new ModelNotaFiscal
                {
                    Cartao = ViewBag.Cartao,
                    Endereco = await endServ.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario")),
                    Produto = prod
                };
                await notaServ.GerarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), notafiscal);
                ProdutoService prodServ = new(_iconfig);

                var produtoRt = await prodServ.RetornaProdutoPorID(item.IdProduto);
                prod[0].Qtd = produtoRt.Qtd - prod[0].Qtd;
                prod[0].Data = null;
                prod[0].Cancelado = null;
                prod[0].ValorTotal = null;

                await prodServ.AlterarProduto(HttpContext.Session.GetString("IdUsuario"), null, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), prod[0]);

                return RedirectToAction("FinalizarPedido", "Conta", new { endereco = ViewBag.RNCUC });
            }

            return View(model);
        }
        [HttpGet("Produto/ComprarProd/{Excluir}")]
        public async Task<IActionResult> AlterarCompraProd(string excluir)
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            if (excluir.Contains("***"))
            {
                string[] alterar = excluir.Split("--");
                string idprod = "";
                string quant = "";
                if (alterar[0] == "unico")
                {
                    var splitExcluir = alterar[1].Split("***");
                    idprod = splitExcluir[0];
                    quant = splitExcluir[1];
                    CompraService Serv = new(_iconfig);
                    await Serv.AlterarItemUnico(HttpContext.Session.GetString("IdUsuario"), quant);
                    return RedirectToAction("ComprarProd", "Produtos", new { produto = idprod, quantia = quant });
                }
                else
                {
                    var splitExcluir = alterar[1].Split("***");
                    idprod = splitExcluir[0];
                    quant = splitExcluir[1];
                    CarrinhoService car = new(_iconfig);
                    var r = await car.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));
                }
                return RedirectToAction("Carrinho", "Produtos");
            }
            return RedirectToAction("Home", "Home");
        }

        public async Task<IActionResult> Carrinho()
        {
            using (GeralService geral = new())
            {
                ViewBag.nomeUser = geral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = geral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            }
            var model = new ModelProduto();
            List<dynamic> ProdutosEqtd = new();


            float? valorTotal = 0;
            int? quantiaTotal = 0;
            List<int?> qtdProdutos = new();

            List<ModelProduto> prod = new();
            CarrinhoService carrinho = new(_iconfig);
            var Produtos = await carrinho.RetornaCarrinho(HttpContext.Session.GetString("IdUsuario"));
            foreach (var idProduto in Produtos)
            {
                var ID = idProduto.Value.IdProduto;
                ProdutoService prodServ = new(_iconfig);


                ModelProduto item = await prodServ.RetornaProdutoPorID(ID) ?? new ModelProduto { };
                valorTotal += item.PrecoProd * idProduto.Value.QtdPorProd;
                quantiaTotal += idProduto.Value.QtdPorProd;

                List<dynamic> unirLista = new();

                unirLista.Add(item);
                unirLista.Add(idProduto.Value.QtdPorProd);
                unirLista.Add(idProduto.Key);

                ProdutosEqtd.Add(unirLista);
            }
            ViewBag.Carrinho = ProdutosEqtd;
            model.ValorTotal = ((float)Math.Round((decimal)valorTotal, 2));
            model.Qtd = quantiaTotal;

            return View(model);
        }
        [HttpGet("Produto/Carrinho/{Excluir}")]
        public async Task<IActionResult> ExcluirCarrinho(string excluir, int quantia)
        {
            CarrinhoService car = new(_iconfig);
            if (excluir.Contains("***"))
            {
                var splitExcluir = excluir.Split("***");
                var idprod = splitExcluir[0];
                var quant = splitExcluir[1];
                var r = await car.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));
                return RedirectToAction("Carrinho", "Produtos");
            }

            await car.DeleteProdCarrinho(HttpContext.Session.GetString("IdUsuario"), excluir);

            return RedirectToAction("Carrinho", "Produtos");
        }

    }
}
