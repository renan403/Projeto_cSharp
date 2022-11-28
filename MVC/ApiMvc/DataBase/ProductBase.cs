using ApiMvc.FireData;
using ApiMvc.Repositories;
using Firebase.Database;


namespace ApiMvc.Database
{
    public class ProductBase : Base, IProductRepository
    {
        public async Task<bool> AddProdutoAsync(string? idUser, ModelProduto model)
        {

            if (idUser != null || idUser == "")
            {
                using Gerador gerador = new();
                var idProd = gerador.AleatoriosProd();
                model.IdProduto = idProd;
                model.IdUser = idUser;
                await _client.Child($"Produtos").PostAsync(model);

                return true;
            }
            return false;
        }
        public async Task<Dictionary<string, ModelProduto>> RetornaArrayProdutosAsync()
        {
            
            var list = (await _client.Child("Produtos").OnceAsync<ModelProduto>()).AsQueryable();
            Dictionary<string, ModelProduto> disc = list.ToDictionary(m => m.Key, m => m.Object) ;
            return disc;
        }
        public async Task<Array> RetornarArrayProdutosVendedorAsync(string idUser)
        {
            return (await _client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdUser == idUser).ToArray();
        }
        public async Task<IEnumerable<FirebaseObject<ModelProduto>>> RetornarObjProdutosVendedorAsync(string idUser)
        {
            return (await _client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdUser == idUser);
        }
        public async Task<bool> DeleteProdutosAsync(string userId)
        {
            try
            {
                var key = await GetUserKey(userId);
                await _client.Child($"Produtos/{key}").DeleteAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        public async Task DeleteUmProdutoAsync(string keyProd)
        {
            await _client.Child($"Produtos/{keyProd}").DeleteAsync();
        }
        public async Task<ModelProduto?> ReturnProductForIDAsync(string idProd)
        {
            if (idProd != "")
            {
                var retorno = (await _client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdProduto == idProd).FirstOrDefault().Object;
                return retorno;
            }
            return null;
        }
        public async Task<bool> AlterarProdutoAsync(string? userId,ModelProduto model)
        {
            try
            {
                var key = await GetUserKey(userId ?? "");
                var objt = await ReturnProductForIDAsync(model.IdProduto ?? "");
                var retorno = (await _client.Child("Produtos").OnceAsync<ModelProduto>()).FirstOrDefault(m => m.Object.IdProduto == model.IdProduto).Key;              
                objt.ImgArquivo = model.ImgArquivo ?? objt.ImgArquivo;
                objt.Categoria = model.Categoria ?? objt.Categoria;
                objt.Data = model.Data ?? objt.Data;
                objt.DescriProd = model.DescriProd ?? objt.DescriProd;
                objt.ValorTotal = model.ValorTotal ?? objt.ValorTotal;
                objt.Cancelado = model.Cancelado ?? objt.Cancelado;
                objt.MarcaProd = model.MarcaProd ?? objt.MarcaProd;
                objt.PrecoProd = model.PrecoProd ?? objt.PrecoProd;
                objt.Qtd = model.Qtd ?? objt.Qtd;
                objt.UrlImg = model.UrlImg ?? objt.UrlImg;
                objt.NomeProd = model.NomeProd ?? objt.NomeProd;
                objt.ModeloProd = model.ModeloProd ?? objt.ModeloProd;
                objt.Fabricante = model.Fabricante ?? objt.Fabricante;
                objt.Path = model.Path ?? objt.Path;

                await _client.Child($"Produtos/{retorno}" ?? "").PatchAsync(objt);
                return true;
            }
            catch (Exception)
            {
                return false;
            }         
        }
    }
}
