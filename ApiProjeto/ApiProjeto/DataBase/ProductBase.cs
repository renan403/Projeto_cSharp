using ApiMvc.FireData;
using ApiMvc.Repositories;
using ApiProjeto.Functions;
using Firebase.Database;


namespace ApiMvc.Database
{
    public class ProductBase : Base, IProductRepository
    {
        public async Task<bool> AddProdutoAsync(string? idUser, IWebHostEnvironment _iweb, ModelProduto model)
        {
            FuncProduto func = new();
            string pwd = Criptografia.Decriptografar(Convert.FromBase64String(model.Pwd));
            string email = Criptografia.Decriptografar(Convert.FromBase64String(model.Email));
            string path = (await RetornarArrayProdutosVendedorAsync(idUser)).Length.ToString();        
            await func.SubirImg(_iweb, model, idUser, 0, email, pwd, path);
            model.Pwd = null;
            model.Email = null;
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
        public async Task<Dictionary<string, ModelProduto>> RetornarObjProdutosVendedorAsync(string idUser)
        {
            return (await _client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdUser == idUser).ToDictionary(m => m.Key,m => m.Object);
            
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
            try
            {
                if (idProd != "")
                {
                    var retorno = (await _client.Child("Produtos").OnceAsync<ModelProduto>()).Where(m => m.Object.IdProduto == idProd).FirstOrDefault().Object;
                    return retorno;
                }
            }
            catch (Exception)
            {

                return null;
            }
            return null;
        }
        public async Task<bool> AlterarProdutoAsync(string? userId, ModelProduto model)
        {
            try
            {

                var key = await GetUserKey(userId ?? "");

                var objt = await ReturnProductForIDAsync(model.IdProduto ?? "");
                var retorno = (await _client.Child("Produtos").OnceAsync<ModelProduto>()).FirstOrDefault(m => m.Object.IdProduto == model.IdProduto).Key;              
                objt.File = model.File ?? objt.File;
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
                objt.Email = null;
                objt.Pwd = null;
                await _client.Child($"Produtos/{retorno}" ?? "").PatchAsync(objt);

                using Auth auth = new();
                await auth.DeleteOneImage(userId, Criptografia.Decriptografar(Convert.FromBase64String(model.Email)), Criptografia.Decriptografar(Convert.FromBase64String(model.Pwd)), model.ImgDel, model.Path);

                return true;
            }
            catch (Exception)
            {
                return false;
            }         
        }
    }
}
