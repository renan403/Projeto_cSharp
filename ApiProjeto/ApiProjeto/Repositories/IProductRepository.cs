using Firebase.Database;

namespace ApiMvc.Repositories

{
    public interface IProductRepository
    {
        Task<Dictionary<string, ModelProduto>> RetornaArrayProdutosAsync();
        Task<Array> RetornarArrayProdutosVendedorAsync(string idUser);
        Task<bool> AddProdutoAsync(string? idUser, IWebHostEnvironment _iweb, ModelProduto model);
        Task<Dictionary<string, ModelProduto>> RetornarObjProdutosVendedorAsync(string idUser);
        Task<bool> DeleteProdutosAsync(string userId);
        Task DeleteUmProdutoAsync(string keyProd);
        Task<ModelProduto?> ReturnProductForIDAsync(string idProd);
        Task<bool> AlterarProdutoAsync(string? userId, ModelProduto model);
    }
}
