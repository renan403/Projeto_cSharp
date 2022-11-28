namespace ApiMvc.Repositories
{
    public interface ICartRepository
    {
        Task<Dictionary<string, ModelProduto>> RetornaCarrinho(string userId);
    }
}
