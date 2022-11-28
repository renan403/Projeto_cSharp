using MVC.Models;
using MVC.Models.Service;
using System;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace MVC.Services
{
    public class EnderecoService : IDisposable
    {
        private bool disposedValue;

        public async Task<ModelEndereco?> RetornarEndPadrao(string urlRtPadrao)
        {

            using HttpClient client = new();
            var response = await client.GetAsync(urlRtPadrao);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ModelEndereco>();
            }
            return null;

        }
        public async Task SalvarEndereco(string urlSaveEnd, ModelEndereco m)
        {
            using HttpClient client = new();
            var Url = new Uri(urlSaveEnd);

            MultipartFormDataContent multiContent = new()
            {
                { new StringContent($"{m.Pais}"), "Pais" },
                { new StringContent($"{m.Cep}"), "Cep" },
                { new StringContent($"{m.Numero.ToString()}"), "Numero" },
                { new StringContent($"{m.UF}"), "UF" },
                { new StringContent($"{m.Cidade}"), "Cidade" },
                { new StringContent($"{m.Endereco}"), "Endereco" },
                { new StringContent($"{m.Padrao}"), "Padrao" },
                { new StringContent($"{m.Bairro}"), "bairro" },
                { new StringContent($"{m.Complemento}"), "Complemento" },
                { new StringContent($"{m.Telefone}"), "Telefone" },
                { new StringContent($"{m.Nome}"), "Nome" },
                { new StringContent($"{m.Estado}"), "Estado" }

            };

            var resp = await client.PostAsync(Url, multiContent);
        }
        public async Task DeletarEndereco(string urlDelete)
        {
            using HttpClient client = new();
            await client.DeleteAsync(urlDelete);
        }
        public async Task<Dictionary<string, ModelEndereco>?> PuxarEnderecos(string urlPuxarEnd)
        {
            using HttpClient client = new();
            return await client.GetFromJsonAsync<Dictionary<string, ModelEndereco>>(urlPuxarEnd);
        }
        public async Task MudarPadrao(string urlMudarPadrao, string key)
        {
            using HttpClient client = new();
            var resp = await client.PutAsync(urlMudarPadrao, new MultipartFormDataContent()
            {
                {new StringContent(key),"key"}
            });
        }
        public async Task AlterarEndereco(string urlAltEnd, ModelEndereco m)
        {
            using HttpClient client = new();
            var Url = new Uri(urlAltEnd);
            MultipartFormDataContent multiContent = new()
            {
                { new StringContent($"{m.Pais}"), "Pais" },
                { new StringContent($"{m.Cep}"), "Cep" },
                { new StringContent($"{m.Numero}"), "Numero" },
                { new StringContent($"{m.UF}"), "UF" },
                { new StringContent($"{m.Cidade}"), "Cidade" },
                { new StringContent($"{m.Endereco}"), "Endereco" },
                { new StringContent($"{m.Padrao}"), "Padrao" },
                { new StringContent($"{m.Bairro}"), "bairro" },
                { new StringContent($"{m.Complemento}"), "Complemento" },
                { new StringContent($"{m.Telefone}"), "Telefone" },
                { new StringContent($"{m.Nome}"), "Nome" },
                { new StringContent($"{m.Estado}"), "Estado" }

            };

            await client.PatchAsync(Url, multiContent);
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
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}