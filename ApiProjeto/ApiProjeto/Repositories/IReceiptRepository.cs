namespace ApiMvc.Repositories
{
    public interface IReceiptRepository
    {
        Task<Dictionary<string, ModelNotaFiscal>> RetornarNotaFiscal(string userId);
        Task<bool> GerarNotaFiscal(string userId, ModelNotaFiscal notaFiscal);
        Task<ModelNotaFiscal> PegarNotaFiscal(string userId, string nota);
        Task ConfirmarPedido(string userId, string nota, string nome);
    }
}
