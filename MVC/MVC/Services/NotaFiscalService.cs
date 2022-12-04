using Firebase.Auth;
using MVC.Models;
using Newtonsoft.Json;

namespace MVC.Services
{
    public class NotaFiscalService : IDisposable
    {
        private readonly IConfiguration _iconfig;
        private bool disposedValue;

        public NotaFiscalService(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }
        public async Task<Dictionary<string, ModelNotaFiscal>?> RetornarNotaFiscal(string userId)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Receipt/ReturnReceipt/{userId}";
                HttpClient client = new();
                var rt = await client.GetAsync(url);
                if (rt.IsSuccessStatusCode)
                    return await rt.Content.ReadAsAsync<Dictionary<string, ModelNotaFiscal>>();
                return null;
            }
            catch (Exception)
            {

                throw;
            }
                         
        }
        public async Task<ModelNotaFiscal?> PegarNotaFiscal(string userId, string nota)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Receipt/GetReceipt/{userId}/{nota}";
                HttpClient client = new();
                var rt = await client.GetAsync(url);
                if (rt.IsSuccessStatusCode)
                    return await rt.Content.ReadAsAsync<ModelNotaFiscal>();
                return null;
            }
            catch (Exception)
            {

                throw;
            }

        }
        public async Task ConfirmarPedido(string userId, string nota,string nome)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Receipt/ConfReceipt/{userId}";
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(nota),"nota" },
                    {new StringContent(nome),"nome"},              
                };
                await client.PostAsync(url,form);
                
            }
            catch (Exception)
            {

                throw;
            }

        } 
        public async Task<bool> GerarNotaFiscal(string userId, ModelNotaFiscal nota)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Receipt/GenerateReceipt/{userId}";
                string jsonConvert = JsonConvert.SerializeObject(nota);
                HttpClient client = new();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(jsonConvert),"notaJson" },
                };
                var rt = await client.PostAsync(url, form);
                if (rt.IsSuccessStatusCode)
                    return true;
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
