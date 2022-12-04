using MVC.Models;
using System.ComponentModel;

namespace MVC.Services
{
    public class CarrinhoService
    {
        private readonly IConfiguration _iconfig;
        public CarrinhoService(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        public async Task<Dictionary<string, ModelProduto>?> RetornaCarrinho(string userId)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/ReturnCart/{userId}";
            HttpClient client = new();
            var rt = await client.GetAsync(url);
            if (rt.IsSuccessStatusCode)
            {
                return await rt.Content.ReadFromJsonAsync<Dictionary<string, ModelProduto>>();
            }
            return null;
        }
        public async Task<bool> SalvarProdCarrinho(string userId, string idprod, int quantidade)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/SaveCart/{userId}";
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(idprod) ,"idProd"},
                    {new StringContent(quantidade.ToString()),"quantidade" }
                };
                var resp = await client.PostAsync(url, form);
                if (resp.IsSuccessStatusCode)
                    return await resp.Content.ReadFromJsonAsync<bool>();
                return false;
            }
            catch (Exception)
            {
                return false;
            }


        }
        public async Task<bool> DeleteProdCarrinho(string userId, string excluir)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/DelProdCar/{userId}/{excluir}";
            try
            {
                HttpClient client = new();
                var rt = await client.DeleteAsync(url);
                if (rt.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> DeletarCarrinho(string userId)
        {

            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/DelCar/{userId}";
                HttpClient client = new();
                var rt = await client.DeleteAsync(url);
                if (rt.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> PossuiNoCarrinho(string userId,string  prodId)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/HaveInTheCar/{userId}/{prodId}";
                HttpClient client = new();
                var rt = await client.GetAsync(url);
                if (rt.IsSuccessStatusCode)
                    return await rt.Content.ReadAsAsync<bool>();
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<string> AddQtdCarrinho(string userId, string idProd, int quantidade)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/AddQtdCar/{userId}";
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(idProd) ,"idProd"},
                    {new StringContent(quantidade.ToString()),"quantidade" }
                };
                var rt = await client.PostAsync(url, form);
                if (rt.IsSuccessStatusCode)
                    return await rt.Content.ReadAsAsync<string>();
                return "erro";
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<bool> AlterarQtdCarrinho(string userId, string idProd, int quantidade)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Cart/AltQtdCar/{userId}";
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(idProd) ,"idProd"},
                    {new StringContent(quantidade.ToString()),"quantidade" }
                };
                var rt = await client.PatchAsync(url, form);
                if (rt.IsSuccessStatusCode)
                    return await rt.Content.ReadAsAsync<bool>();
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}