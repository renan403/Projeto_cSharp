using ApiMvc.FireData;
using ApiMvc.Repositories;
using Firebase.Database;
using Microsoft.EntityFrameworkCore;

namespace ApiMvc.Database
{
    public class CartBase : Base ,ICartRepository
    {
 

        public async Task<Dictionary<string, ModelProduto>> RetornaCarrinho(string userId)
        {
            var KeyUser = await GetUserKey(userId);
            return  ((await _client.Child($"Usuarios/{KeyUser}/Carrinho").OnceAsync<ModelProduto>()).AsQueryable()).ToDictionary(m =>m.Key, n=>n.Object);

        }
        public async Task<bool> SalvarProdCarrinho(string userId, string idprod, int quantidade)
        {
            var KeyUser = await GetUserKey(userId);
            var count = await _client.Child($"Usuarios/{KeyUser}/Carrinho").OnceAsync<ModelProduto>();
            var carrinho = count.ToArray();

            for (int i = 0; i <= carrinho.Length; i++)
            {
                try
                {
                    if (!carrinho[i].Key.Contains($"Produto{i + 1}"))
                    {
                        await _client.Child($"Usuarios/{KeyUser}/Carrinho/Produto{i + 1}").PatchAsync(new ModelProduto { IdProduto = idprod, QtdPorProd = quantidade });
                        return true;
                    }

                }
                catch
                {
                    await _client.Child($"Usuarios/{KeyUser}/Carrinho/Produto{i + 1}").PatchAsync(new ModelProduto { IdProduto = idprod, QtdPorProd = quantidade });
                    return true;
                }

            }
            return false;
        }
        public async Task<bool> DeleteProdCarrinho(string Key, string userId)
        {
            if (Key.Contains("Produto") && userId != null)
            {
                var chave = await GetUserKey(userId);
                await _client.Child($"Usuarios/{chave}/Carrinho/{Key}").DeleteAsync();
                return true;
            }
            else
            {
                return false;
            }

        }
        public async Task<bool> AlterarQtdCarrinho(string userId, string idProd, int quantidade)
        {
            var carrinho = await RetornaCarrinho(userId);

            foreach (var car in carrinho)
            {
                if (car.Value.IdProduto == idProd)
                {
                    var key = await GetUserKey(userId);
                    await _client.Child($"Usuarios/{key}/Carrinho/{car.Key}").PatchAsync(new ModelProduto
                    {
                        IdProduto = car.Value.IdProduto,
                        QtdPorProd = quantidade
                    });
                    return true;
                }
            }
            return false;
        }
        public async Task<string> AddQtdCarrinho(string userId, string idProd, int quantidade)
        {
            var carrinho = await RetornaCarrinho(userId);

            foreach (var car in carrinho)
            {
                if (car.Value.IdProduto == idProd)
                {
                    if (car.Value.QtdPorProd + quantidade > 5)
                    {
                        return $"{car.Value.QtdPorProd}";
                    }
                    var key = await GetUserKey(userId);
                    await _client.Child($"Usuarios/{key}/Carrinho/{car.Key}").PatchAsync(new ModelProduto
                    {
                        IdProduto = car.Value.IdProduto,
                        QtdPorProd = car.Value.QtdPorProd + quantidade
                    });
                    return "sucesso";
                }
            }
            return "erro";
        }
        public async Task<bool> PossuiNoCarrinho(string idClient, string idProd)
        {
            var carrinho = await RetornaCarrinho(idClient);

            foreach (var car in carrinho)
            {
                if (car.Value.IdProduto == idProd)
                {
                    return true;
                }
            }
            return false;
        }
        public async Task DeletarCarrinho(string userId)
        {
            var key = await GetUserKey(userId);
            await _client.Child($"Usuarios/{key}/Carrinho").DeleteAsync();
        }
    }
}
