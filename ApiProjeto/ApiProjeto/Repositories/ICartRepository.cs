namespace ApiMvc.Repositories
{
    public interface ICartRepository
    {
        Task<Dictionary<string, ModelProduto>> RetornaCarrinho(string userId);
        Task<bool> SalvarProdCarrinho(string userId, string idprod, int quantidade);
        Task<bool> DeleteProdCarrinho(string Key, string userId);
        Task<bool> AlterarQtdCarrinho(string userId, string idProd, int quantidade);
        Task<string> AddQtdCarrinho(string userId, string idProd, int quantidade);
        Task<bool> PossuiNoCarrinho(string idClient, string idProd);
        Task DeletarCarrinho(string userId);
    }
}
