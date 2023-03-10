using MVC.Models;

namespace MVC.Repository
{
    public interface ICarrinhoService
    {
        Task<Dictionary<string, ModelProduto>?> RetornaCarrinho(string userId);
        Task<bool> SalvarProdCarrinho(string userId, string idprod, int quantidade);
        Task<bool> DeleteProdCarrinho(string userId, string excluir);
        Task<bool> DeletarCarrinho(string userId);
        Task<bool> PossuiNoCarrinho(string userId, string prodId);
        Task<string> AddQtdCarrinho(string userId, string idProd, int quantidade);
        Task<bool> AlterarQtdCarrinho(string userId, string idProd, int quantidade);

    }
}
