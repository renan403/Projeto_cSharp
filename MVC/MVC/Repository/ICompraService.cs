using MVC.Models;

namespace MVC.Repository
{
    public interface ICompraService
    {
        Task AddItemUnico(string userId, ModelProduto prod);
        Task AlterarItemUnico(string userId, string qtd);
        Task<ModelProduto?> PegarProduto(string userId);
    }
}
