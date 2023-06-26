using MVC.Models;
using MVC.Repository;

namespace MVC.Services
{
    public class CompraService :UrlBase, IDisposable , ICompraService
    {
        private bool disposedValue;
        

        public async Task AddItemUnico(string userId, ModelProduto prod)
        {
            string url = $"{_url}Purchase/AddOneItem/{userId}";
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
                {new StringContent(prod.PrecoProdStr ?? ""),"PrecoProdStr"},
                {new StringContent(prod.Qtd.ToString() ?? ""),"Qtd"},
                {new StringContent(prod.QtdPorProd.ToString() ?? ""),"QtdPorProd"},
                {new StringContent(prod.UrlImg ?? ""),"UrlImg"},
                {new StringContent(prod.ValorTotal.ToString() ?? ""),"ValorTotal"},
                {new StringContent(prod.ValorTotalStr ?? ""),"ValorTotalStr"},
            };
            await client.PostAsync(url, content);
        }

        public async Task AlterarItemUnico(string userId, string qtd)
        {
            string url = $"{_url}Purchase/ChangeSingleItem/{userId}";
                HttpClient client = new();
            MultipartFormDataContent content = new()
            {
                {new StringContent(qtd),"quantidade"},
            };
            await client.PatchAsync(url, content);
        }
        public async Task<ModelProduto?> PegarProduto(string userId)
        {
            string url = $"{_url}Purchase/PickUpProduct/{userId}";
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
