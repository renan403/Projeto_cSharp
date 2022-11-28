using Firebase.Database;
using MVC.Models;
using MVC.Models.Service;
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

        public async Task DeletarProdAsync(string uri)
        {
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(uri);
                await client.DeleteAsync(uri);
            }
        }
        public async Task<ModelProduto?> RetornaProdutoPorID(string uriProdId)
        {
            using (HttpClient client = new())
            {             
               var resp = await client.GetAsync(uriProdId);
                if (resp.IsSuccessStatusCode)
                {
                    return await resp.Content.ReadFromJsonAsync<ModelProduto>();
                    
                }
                return null;
            }
        }
        public async Task<Dictionary<string, ModelProduto>?> RetornarArrayProdutosVendedor(string uri)
        {
            using(HttpClient httpClient = new())
            {
                var Url = new Uri(uri);
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
        public async Task<HttpResponseMessage> AlterarProduto(string idUser,string? img, string email, string pwd, ModelProduto p, string uri)
        {
            using (HttpClient client = new())
            {
                var Url = new Uri(uri);
                var ch = Chave.GetKey();
                MultipartFormDataContent multiContent = new()
                {
                    { new StringContent(idUser), "IdUser" },
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
        public async Task<HttpResponseMessage> VendaNoApp(string idUser, string email, string pwd, ModelProduto p, string uri)
        {
            using (ProdutoService prod = new())
            {
                HttpClient client = new();
                var Url = new Uri(uri);
                byte[] file;
                using (var br = new BinaryReader(p.File.OpenReadStream()))
                {
                    file = br.ReadBytes((int)p.File.OpenReadStream().Length);
                }
                var ch = Chave.GetKey();
                var multiContent = new MultipartFormDataContent
                {
                    { new StringContent(idUser), "IdUser" },
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
