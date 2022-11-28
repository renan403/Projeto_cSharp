namespace ApiMvc.Repositories
{
    public interface IPurchaseRepository
    {
        Task AddItemUnico(string idUser, ModelProduto produto);
        Task AlterarItemUnico(string userId, int qtd);
        Task<ModelProduto> PegarProduto(string userId);
    }
}
