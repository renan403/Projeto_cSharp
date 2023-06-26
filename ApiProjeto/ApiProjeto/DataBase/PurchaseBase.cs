using ApiMvc.FireData;
using ApiMvc.Repositories;

namespace ApiMvc.Database
{
    public class PurchaseBase : Base, IPurchaseRepository
    {
        public async Task AddItemUnico(string userId, ModelProduto produto)
        {
            var key = await GetUserKey(userId);
            await _client.Child($"Usuarios/{key}/Produto/").PatchAsync(produto);
            var pBase = new ProductBase();
            var prod = pBase.ReturnProductForIDAsync(produto.IdProduto);

        }
        public async Task<ModelProduto> PegarItemUnico(string key)
        {
            return (await _client.Child($"Usuarios/{key}/Produto/").OnceSingleAsync<ModelProduto>());
        }
        public async Task AlterarItemUnico(string userId, int qtd)
        {
            var key = await GetUserKey(userId);
            var item = await PegarItemUnico(key);
            item.ValorTotal = (float)Math.Round(((decimal)item.PrecoProd * qtd), 2); ;
            item.ValorTotalStr = ((decimal)item.ValorTotal).ToString("N2"); ;
            item.QtdPorProd = qtd;
            await AddItemUnico(userId, item);
        }
        public async Task<ModelProduto> PegarProduto(string userId)
        {
            var key = await GetUserKey(userId);
            ModelProduto model = (await _client.Child($"Usuarios/{key}/Produto/").OnceSingleAsync<ModelProduto>());
            return model;
        }
    }
}
