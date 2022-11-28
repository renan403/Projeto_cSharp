namespace ApiMvc.Repositories
{
    public interface IPurchaseRepository
    {
        Task AddItemUnico(string userId, ModelProduto produto);
        Task<ModelProduto> PegarItemUnico(string key);
        Task AlterarItemUnico(string userId, int qtd);
        Task<ModelProduto> PegarProduto(string userId);
    }
}
