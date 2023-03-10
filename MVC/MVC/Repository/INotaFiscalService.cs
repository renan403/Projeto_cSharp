using MVC.Models;

namespace MVC.Repository
{
    public interface INotaFiscalService
    {
        Task<Dictionary<string, ModelNotaFiscal>?> RetornarNotaFiscal(string userId);
        Task<ModelNotaFiscal?> PegarNotaFiscal(string userId, string nota);
        Task ConfirmarPedido(string userId, string nota, string nome);
        Task<bool> GerarNotaFiscal(string userId, ModelNotaFiscal nota);
    }
}
