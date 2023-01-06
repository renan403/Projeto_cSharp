using Microsoft.AspNetCore.DataProtection.KeyManagement;
using MVC.Models;
using Newtonsoft.Json;
using System.Security.Policy;

namespace MVC.Services
{
    public class CartaoService : IDisposable
    {
        private readonly IConfiguration _iconfig;
        private bool disposedValue;

        public CartaoService(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }

        public async Task<string> SalvarCartao(string userId, ModelCartao cartao)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/SaveCard/{userId}";
                HttpClient client = new();

                var jsonCartao = JsonConvert.SerializeObject(cartao);

                MultipartFormDataContent form = new()
                {
                    {new StringContent(jsonCartao),"jsonCartao" },
                };
                var resp = await client.PostAsync(url, form);               
                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadAsStringAsync();
                }
                if ((await resp.Content.ReadAsStringAsync()).ToLower().Contains("bad request"))
                {
                    return "Cartão não adicionado, tente mais tarde ou entre em contato com suporte";
                }
                return "erro";
            }
            catch (Exception)
            {

                throw;
            }

            
        }
        public async Task<Dictionary<string, ModelCartao>?> ReturnCard(string userId)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/ReturnCard/{userId}";
            try
            {
                HttpClient client = new();
                var rp = await client.GetAsync(url);
                if (rp.IsSuccessStatusCode)
                  return await rp.Content.ReadAsAsync<Dictionary<string, ModelCartao>>();
                return null;               
            }
            catch (Exception)
            {
                throw;
            }
            
        }
        public async Task<bool> DeleteCartao(string userId, string cartao)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/DeleteCard/{userId}/{cartao}";
            try
            {
                HttpClient client = new();
                var rp = await client.DeleteAsync(url);
                if (rp.IsSuccessStatusCode)
                    return await rp.Content.ReadAsAsync<bool>();
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Dictionary<string,ModelCartao>?> RetornarCartaoPadrao(string userId)
        {
            HttpClient client = new();
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/ReturnPatternCard/{userId}";            
            var rp = await client.GetAsync(url);
            if (rp.IsSuccessStatusCode)
                return await rp.Content.ReadAsAsync<Dictionary<string, ModelCartao>>();
            return null;
            
        }
        public async Task<bool> MudarPadraoCard(string userId, string key)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/ChangePatternCard/{userId}";
            try
            {
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(key),"key" },
                };
                var rp = await client.PatchAsync(url,form);
                if (rp.IsSuccessStatusCode)
                    return await rp.Content.ReadAsAsync<bool>();
                return false;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<ModelCartao?> Cartao(string userId, string keyCard)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/Card/{userId}/{keyCard}";
            try
            {
                HttpClient client = new();             
                var rt = await client.GetAsync(url);
                if (rt.IsSuccessStatusCode)
                    return await rt.Content.ReadFromJsonAsync<ModelCartao>();
                return null;
            }
            catch (Exception)
            {

                throw;
            }
        }
        public async Task<bool> AlterarCartao(string userId, string cartao, string nome, string data)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Card/AlterCard/{userId}";
            try
            {
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(cartao),"cartao" },
                    {new StringContent(nome),"nome" },
                    {new StringContent(data),"data" },
                };
                var rp = await client.PatchAsync(url, form);
                if (rp.IsSuccessStatusCode)
                    return await rp.Content.ReadAsAsync<bool>();
                return false;
            }
            catch (Exception)
            {
                throw;
            }
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

        public void Dispose()
        {
            // Não altere este código. Coloque o código de limpeza no método 'Dispose(bool disposing)'
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
