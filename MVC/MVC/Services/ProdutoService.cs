using Firebase.Database;
using MVC.Models;
using MVC.Services.Funcoes;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http.Json;
using System.Security.Policy;
using static System.Net.WebRequestMethods;


namespace MVC.Services
{
    public class ProdutoService : IDisposable
    {
        private bool disposedValue;
        private readonly IConfiguration _iconfig;
        public ProdutoService(IConfiguration iconfig)
        {       
            _iconfig = iconfig;
        }

        public async Task DeletarProdAsync(string keyProd)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Product/DeleteOneProd/{keyProd}";
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(url);
                await client.DeleteAsync(url);
            }
        }
        public async Task<ModelProduto?> RetornaProdutoPorID(string prodId)
        {
            string url = $"{_iconfig.GetValue<string>("UrlApi")}Product/ReturnProd/{prodId}";
            using (HttpClient client = new())
            {             
               var resp = await client.GetAsync(url);
                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadFromJsonAsync<ModelProduto>();                  
                }
                return null;
            }
        }
        public async Task<Dictionary<string, ModelProduto>?> RetornarArrayProdutosVendedor(string userId)
        {
            using(HttpClient httpClient = new())
            {
                var Url = new Uri($"{_iconfig.GetValue<string>("UrlApi")}Product/GetProdSeller/{userId}");
                var response = await httpClient.GetAsync(Url);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadFromJsonAsync<Dictionary<string, ModelProduto>>();
                    }
                    catch (Exception e)
                    {
                        return null;
                    }                  
                }
                return null;
            }
        }
        public async Task<HttpResponseMessage> AlterarProduto(string userId,string? img, string email, string pwd, ModelProduto p)
        {
            using (HttpClient client = new())
            {
                var Url = new Uri($"{_iconfig.GetValue<string>("UrlApi")}Product/AlterProd/{userId}");
                var ch = Chave.GetKey();
                MultipartFormDataContent multiContent = new()
                {
                    { new StringContent(userId), "IdUser" },
                    { new StringContent($"{Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))}"), "Pwd" },
                    { new StringContent($"{Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))}"), "Email" },
                    { new StringContent($"{p.IdProduto}"), "IdProduto" },
                    { new StringContent($"{p.NomeVendedor}"), "NomeVendedor" },
                    { new StringContent($"{p.DescriProd}"), "DescriProd" },
                    { new StringContent($"{p.MarcaProd}"), "MarcaProd" },
                    { new StringContent($"{p.ModeloProd}"), "ModeloProd" },
                    { new StringContent($"{p.Categoria}"), "Categoria" },
                    { new StringContent($"{p.Fabricante}"), "Fabricante" },
                    { new StringContent($"{p.NomeProd}"), "NomeProd" },
                    { new StringContent($"{p.PrecoProd}"), "PrecoProd" },
                    { new StringContent($"{p.Qtd}"), "Qtd" }
                };
                if (p.File != null)
                {
                    if (img != null)
                    {
                        byte[] file;
                        using (var br = new BinaryReader(p.File.OpenReadStream()))
                        {
                            file = br.ReadBytes((int)p.File.OpenReadStream().Length);
                        }
                        multiContent.Add(new StringContent($"{img}"), "imgDel");
                        multiContent.Add(new ByteArrayContent(file), "file", p.File.FileName);
                    }
                }
                
                return await client.PatchAsync(Url, multiContent);
            }
        }
        public async Task<HttpResponseMessage> VendaNoApp(string userId, string email, string pwd, ModelProduto p)
        {
          
                HttpClient client = new();
                var Url = new Uri($"{_iconfig.GetValue<string>("UrlApi")}Product/{userId}");
                byte[] file;
                using (var br = new BinaryReader(p.File.OpenReadStream()))
                {
                    file = br.ReadBytes((int)p.File.OpenReadStream().Length);
                }
                var ch = Chave.GetKey();
                var multiContent = new MultipartFormDataContent
                {
                    { new StringContent(userId), "IdUser" },
                    { new StringContent($"{Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))}"), "Pwd" },
                    { new StringContent($"{Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))}"), "Email" },
                    { new ByteArrayContent(file), "file", p.File.FileName },
                    { new StringContent($"{p.NomeVendedor}"), "NomeVendedor" },
                    { new StringContent($"{p.DescriProd}"), "DescriProd" },
                    { new StringContent($"{p.MarcaProd}"), "MarcaProd" },
                    { new StringContent($"{p.ModeloProd}"), "ModeloProd" },
                    { new StringContent($"{p.Categoria}"), "Categoria" },
                    { new StringContent($"{p.Fabricante}"), "Fabricante" },
                    { new StringContent($"{p.NomeProd}"), "NomeProd" },
                    { new StringContent($"{p.PrecoProd}"), "PrecoProd" },
                    { new StringContent($"{p.Qtd}"), "Qtd" }
                };

                return await client.PostAsync(Url, multiContent);
            
        }
        public static string PegarNomeUrl(string url)
        {
            string separar = url;
            string[] separar1 = separar.Split("%2F");
            string[] separar2 = separar1[3].Split("?");
            return separar2[0];
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
