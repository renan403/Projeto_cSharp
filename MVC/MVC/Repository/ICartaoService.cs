using MVC.Models;

namespace MVC.Repository
{
    public interface ICartaoService
    {
        Task<string> SalvarCartao(string userId, ModelCartao cartao);
        Task<Dictionary<string, ModelCartao>?> ReturnCard(string? userId);
        Task<bool> DeleteCartao(string userId, string cartao);
        Task<Dictionary<string, ModelCartao>?> RetornarCartaoPadrao(string userId);
        Task<bool> MudarPadraoCard(string userId, string key);
        Task<ModelCartao?> Cartao(string userId, string keyCard);
        Task<bool> AlterarCartao(string userId, string cartao, string nome, string data);

    }
}
