﻿using MVC.Models;
using MVC.Repository;
using Newtonsoft.Json;

namespace MVC.Services
{
    public class EnderecoService :UrlBase, IDisposable , IEnderecoService
    {
        private bool disposedValue;
        

        public async Task<ModelEndereco?> RetornarEndPadrao(string? userId)
        {
            if (string.IsNullOrEmpty(userId))
                return null;
            string url = $"{_url}Address/ReturnPattern/{userId}";
            using HttpClient client = new();
            var response = await client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ModelEndereco>();
            }
            return null;
        }
        public async Task SalvarEndereco(string userId, ModelEndereco m)
        {
            using HttpClient client = new();
            var Url = new Uri($"{_url}Address/SaveAddress/{userId}");

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
        public async Task DeletarEndereco(string userId, string opcao)
        {
            string url = $"{_url}Address/DeleteAddress/{userId}/{opcao}";
            using HttpClient client = new();
            await client.DeleteAsync(url);
        }
        public async Task<Dictionary<string, ModelEndereco>?> PuxarEnderecos(string userId)
        {
            string url = $"{_url}Address/PullAddress/{userId}";
            using HttpClient client = new();
            return await client.GetFromJsonAsync<Dictionary<string, ModelEndereco>>(url);
        }
        public async Task MudarPadrao(string userId, string key)
        {
            string url = $"{_url}Address/ChangeDefault/{userId}";
            using HttpClient client = new();
            var resp = await client.PutAsync(url, new MultipartFormDataContent()
            {
                {new StringContent(key),"key"}
            });
        }
        public async Task AlterarEndereco(string userId,string key, ModelEndereco m)
        {
            string url = $"{_url}Address/AlterAddress/{userId}/{key}";
            using HttpClient client = new();
            var Url = new Uri(url);
            var jsonEndereco = JsonConvert.SerializeObject(m);
            MultipartFormDataContent multiContent = new()
            {
                { new StringContent($"{jsonEndereco}"), "jsonEndereco" },              
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