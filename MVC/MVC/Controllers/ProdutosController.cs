using Microsoft.AspNetCore.Mvc;
using MVC.Models;
using MVC.Services;
using System.Runtime.Intrinsics.Arm;
using MVC.Funcoes;
using MVC.Repository;


namespace MVC.Controllers
{
    public class ProdutosController : Controller
    {
        private readonly IConfiguration _iconfig;
        private readonly IGeralService _IGeral;
        private readonly ICartaoService _cartao;
        private readonly IEnderecoService _endereco;
        private readonly ICompraService _compra;
        private readonly ICarrinhoService _carrinho;
        private readonly INotaFiscalService _notaFiscal;
        private readonly IProdutoService _produto;

        public ProdutosController(IConfiguration iconfig, IGeralService geralService, ICartaoService cartao, ICompraService compra, ICarrinhoService carrinho, INotaFiscalService notaFiscal, IEnderecoService enderecoService, IProdutoService produto)
        {
            this._iconfig = iconfig;
            this._IGeral = geralService;
            this._cartao = cartao;
            _compra = compra;
            _carrinho = carrinho;
            _notaFiscal = notaFiscal;
            _endereco = enderecoService;
            _produto = produto;
        }

        [HttpGet("Produtos/Produtos/{prodId}")]
        public async Task<IActionResult> Produtos(string prodId)
        {
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");
            
            ViewBag.Logado = HttpContext.Session.GetString("IdUsuario") == null || HttpContext.Session.GetString("IdUsuario") == "" ? false : true;
            string IdProduto = prodId;
            ModelProduto? model = new();
            if (prodId.Contains("Erro***"))
            {
                var urlErro = prodId.IndexOf("***");

                IdProduto = prodId[(urlErro + 3)..];



                model = await _produto.RetornaProdutoPorID(IdProduto);
                var quantia = prodId[0].ToString();
                model.ErroProd = $"Máximo até 5 produtos iguais no carrinho, quantidade atual {quantia}.";
                ViewBag.pagProduto = model;
                TempData["prodId"] = IdProduto;
                return View(model);

            }
            try
            {
                ViewBag.pagProduto = model = await _produto.RetornaProdutoPorID(IdProduto);
            }
            catch (Exception)
            {

                ViewBag.pagProduto = model = new ModelProduto();
            }

            TempData["prodId"] = IdProduto;

            return View(model);
        }
        [HttpGet]
        public async Task<PartialViewResult> PartialProdutos()
        {

            return PartialView();
        }

        [HttpGet("/Produtos/PartialQuantidade/{prodId}")]
        public async Task<ActionResult> PartialQuantidade(string prodId)
        {
 
            var model = await _produto.RetornaProdutoPorID(prodId) ?? new ModelProduto()
            {
                Qtd = 0
            }; 

            return PartialView(model);
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


                        var jaPossuiNoCarrinho = await _carrinho.PossuiNoCarrinho(HttpContext.Session.GetString("IdUsuario"), idP);
                        if (jaPossuiNoCarrinho)
                        {
                            string resp = await _carrinho.AddQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idP, select);
                            if (resp == "sucesso")
                            {
                                return RedirectToAction("Carrinho", "Produtos");
                            }
                            return RedirectToAction("Produtos", "Produtos", new { prodId = $"{resp}Erro***{idP}" });
                        }


                        var result = await _carrinho.SalvarProdCarrinho(HttpContext.Session.GetString("IdUsuario"), idP, select);


                        return RedirectToAction("Carrinho", "Produtos");

                    case "Comprar":
                        ModelProduto produto = await _produto.RetornaProdutoPorID(idP);
                        produto.Qtd = select;
                        produto.Cancelado = false;
                        produto.Data = DateTime.Now.ToString("dd/MM/yyyy");
                        produto.ValorTotal = (float)Math.Round(((decimal)produto.PrecoProd * select), 2);
                        await _compra.AddItemUnico(HttpContext.Session.GetString("IdUsuario"), produto);

                        return RedirectToAction("ComprarProd", "Produtos", new { produto = idP, quantia = select });
                }

            }
            return RedirectToAction("Error", "Home");

        }
        [HttpGet("Produtos/ComprarProd/{produto}")]
        public async Task<IActionResult> ComprarProd(string produto, int quantia, string cardTrocar, string finalizar)
        {

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");

            var model = new ModelProduto();
            List<dynamic> ProdutosEqtd = new();
            List<ModelProduto> prod = new();

            List<int?> qtdProdutos = new();


            ModelProduto? item = await _compra.PegarProduto(HttpContext.Session.GetString("IdUsuario"));
            List<dynamic> unirLista = new();

            unirLista.Add(item);
            unirLista.Add(quantia);
            unirLista.Add(item.IdProduto ?? "");

            ProdutosEqtd.Add(unirLista);
            ViewBag.Carrinho = ProdutosEqtd;
            model.ValorTotal = item.ValorTotal;
            model.Qtd = item.Qtd;


            if (cardTrocar == null)
                ViewBag.Cartao = await _cartao.Cartao(HttpContext.Session.GetString("IdUsuario"), HttpContext.Session.GetString("cartao"));
            
            else
                ViewBag.Cartao = await _cartao.Cartao(HttpContext.Session.GetString("IdUsuario"), cardTrocar);
             ViewBag.ArrayCartoes = await _cartao.ReturnCard(HttpContext.Session.GetString("IdUsuario"));
            if (!string.IsNullOrEmpty(finalizar))
            {
                prod.Add(item);
                var notafiscal = new ModelNotaFiscal
                {
                    Cartao = ViewBag.Cartao,
                    Endereco = await _endereco.RetornarEndPadrao(HttpContext.Session.GetString("IdUsuario")),
                    Produto = prod
                };
                await _notaFiscal.GerarNotaFiscal(HttpContext.Session.GetString("IdUsuario"), notafiscal);

                var produtoRt = await _produto.RetornaProdutoPorID(item.IdProduto);
                prod[0].Qtd = produtoRt.Qtd - prod[0].Qtd;
                prod[0].Data = null;
                prod[0].Cancelado = null;
                prod[0].ValorTotal = null;

                await _produto.AlterarProduto(HttpContext.Session.GetString("IdUsuario"), null, HttpContext.Session.GetString("SessaoEmail"), HttpContext.Session.GetString("Senha"), prod[0]);

                return RedirectToAction("FinalizarPedido", "Conta", new { endereco = ViewBag.RNCUC });
            }

            return View(model);
        }
        [HttpGet]
        public async Task<PartialViewResult> PartialAlterarCarrinho(string Prod)
       { 
            await _carrinho.DeleteProdCarrinho(HttpContext.Session.GetString("IdUsuario"), Prod);
            var retorno = await _produto.RetornoProdModel(_carrinho, HttpContext.Session.GetString("IdUsuario"));
            ViewBag.Carrinho = retorno.Item2;
            return PartialView("PartialAlterarCarrinho", retorno.Item1);
        }
        [HttpGet("Produto/ComprarProd/{Excluir}")]
        public async Task<IActionResult> AlterarCompraProd(string excluir)
        {
  
                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");

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
                    await _compra.AlterarItemUnico(HttpContext.Session.GetString("IdUsuario"), quant);
                    return RedirectToAction("ComprarProd", "Produtos", new { produto = idprod, quantia = quant });
                }
                else
                {
                    var splitExcluir = alterar[1].Split("***");
                    idprod = splitExcluir[0];
                    quant = splitExcluir[1];
                    var r = await _carrinho.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));
                }
                return RedirectToAction("Carrinho", "Produtos");
            }
            return RedirectToAction("Home", "Home");
        }

        public async Task<IActionResult> Carrinho()
        {

                ViewBag.nomeUser = _IGeral.RetornaNomeNull(HttpContext.Session.GetString("nomeFormatado") ?? "");
                ViewBag.RNCUC = _IGeral.RetornoRNCUC(HttpContext.Session.GetString("Endereço") ?? "");

            var model = new ModelProduto();
            List<dynamic> ProdutosEqtd = new();


            float? valorTotal = 0;
            int? quantiaTotal = 0;
            List<int?> qtdProdutos = new();

            List<ModelProduto> prod = new();
            var Produtos = await _carrinho.RetornaCarrinho(HttpContext.Session.GetString("IdUsuario"));
            foreach (var idProduto in Produtos)
            {
                var ID = idProduto.Value.IdProduto;
                var qtdBase = (await _produto.RetornaProdutoPorID(ID)).Qtd;
                idProduto.Value.QtdPorProd = qtdBase <= idProduto.Value.QtdPorProd ? qtdBase : idProduto.Value.QtdPorProd;
                ModelProduto item = await _produto.RetornaProdutoPorID(ID) ?? new ModelProduto { };
                valorTotal += item.PrecoProd * idProduto.Value.QtdPorProd;
                quantiaTotal += idProduto.Value.QtdPorProd;

                List<dynamic> unirLista = new()
                {
                    item,
                    idProduto.Value.QtdPorProd,
                    idProduto.Key
                };

                ProdutosEqtd.Add(unirLista);
            }
            ViewBag.Carrinho = ProdutosEqtd;
            model.ValorTotal = Math.Round((double)valorTotal, 2);
            model.Qtd = quantiaTotal;

            return View(model);
        }
        [HttpGet("Produto/Carrinho/{Excluir}")]
        public async Task<IActionResult> ExcluirCarrinho(string excluir, int quantia)
        {
            if (excluir.Contains("***"))
            {
                var splitExcluir = excluir.Split("***");
                var idprod = splitExcluir[0];
                var quant = splitExcluir[1];
                var r = await _carrinho.AlterarQtdCarrinho(HttpContext.Session.GetString("IdUsuario"), idprod, int.Parse(quant));
                return RedirectToAction("Carrinho", "Produtos");
            }

            await _carrinho.DeleteProdCarrinho(HttpContext.Session.GetString("IdUsuario"), excluir);

            return RedirectToAction("Carrinho", "Produtos");
        }

    }
}
