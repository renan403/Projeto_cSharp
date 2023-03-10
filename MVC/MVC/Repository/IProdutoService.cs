using MVC.Models;
using MVC.Services;

namespace MVC.Repository
{
    public interface IProdutoService
    {
        Task DeletarProdAsync(string keyProd);
        Task<ModelProduto?> RetornaProdutoPorID(string prodId);
        Task<Dictionary<string, ModelProduto>?> RetornarArrayProdutosVendedor(string userId);
        Task<HttpResponseMessage> AlterarProduto(string userId, string? img, string email, string pwd, ModelProduto p);
        Task<HttpResponseMessage> VendaNoApp(string userId, string email, string pwd, ModelProduto p);
        string PegarNomeUrl(string url);
        Task<Result> FinalizarCompra(Dictionary<string, ModelProduto> model);
        Task<Tuple<ModelProduto, List<dynamic>>> RetornoProdModel(ICarrinhoService _carrinho, string idUser);
    }
}
