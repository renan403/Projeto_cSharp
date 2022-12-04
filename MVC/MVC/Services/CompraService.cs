using Firebase.Auth;
using FirebaseAdmin.Auth;
using MVC.Models;

namespace MVC.Services
{
    public class CompraService : IDisposable
    {
        private bool disposedValue;

        private readonly IConfiguration _iconfig;
        public CompraService(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        public async Task AddItemUnico(string userId, ModelProduto prod)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Purchase/PickUpProduct/{userId}";
                HttpClient client = new();
            MultipartFormDataContent content = new()
            {
                {new StringContent((prod.Cancelado).ToString() ?? ""),"Cancelado"},
                {new StringContent(prod.Categoria ?? ""),"Categoria"},
                {new StringContent(prod.Data ?? ""),"Data"},
                {new StringContent(prod.DescriProd ?? ""),"DescriProd"},
                {new StringContent(prod.Fabricante ?? ""),"Fabricante"},
                {new StringContent(prod.IdProduto ?? ""),"IdProduto"},
                {new StringContent(prod.IdUser ?? ""),"IdUser"},               
                {new StringContent(prod.MarcaProd ?? ""),"MarcaProd"},
                {new StringContent(prod.ModeloProd ?? ""),"ModeloProd"},
                {new StringContent(prod.NomeProd ?? ""),"NomeProd"},
                {new StringContent(prod.NomeVendedor ?? ""),"NomeVendedor"},
                {new StringContent(prod.Path ?? ""),"Path"},
                {new StringContent(prod.PrecoProd.ToString() ?? ""),"PrecoProd"},
                {new StringContent(prod.Qtd.ToString() ?? ""),"Qtd"},
                {new StringContent(prod.UrlImg ?? ""),"UrlImg"},
                {new StringContent(prod.ValorTotal.ToString() ?? ""),"ValorTotal"},
            };
            await client.PostAsync(url, content);
        }

        public async Task AlterarItemUnico(string userId, string qtd)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Purchase/AddOneItem/{userId}";
                HttpClient client = new();
            MultipartFormDataContent content = new()
            {
                {new StringContent(qtd),"quantidade"},
            };
            await client.PatchAsync(url, content);
        }
        public async Task<ModelProduto?> PegarProduto(string userId)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Purchase/PickUpProduct/{userId}";
            HttpClient client = new();           
            var prod = await client.GetAsync(url);
            if (prod.IsSuccessStatusCode)
            {
               return await prod.Content.ReadFromJsonAsync<ModelProduto>();
            }
            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                }
                disposedValue = true;
            }
        }
        void IDisposable.Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
