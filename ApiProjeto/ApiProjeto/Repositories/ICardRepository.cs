namespace ApiMvc.Repositories
{
    public interface ICardRepository
    {
        Task<string> SalvarCard(string userId, ModelCartao model);
        Task<Dictionary<string, ModelCartao>> ReturnCard(string userId);
        Task<Dictionary<string, ModelCartao>?> RetornarCartaoPadrao(string userId);
        Task<bool> AlterarCard(string userId, string cartao, string nome, string data);
        Task<ModelCartao> Cartao(string userId, string keyCard);
        Task<bool> VerificaCardPadrao(string userId);
        Task<bool> MudarPadraoCard(string userId, string key);
        Task<bool> DeleteCard(string userId, string cartao);
    }
}
