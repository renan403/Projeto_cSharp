using Firebase.Database;
using MVC.Funcoes;
using MVC.Models;
using MVC.Repository;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Net.Http.Json;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Policy;
using static System.Net.WebRequestMethods;


namespace MVC.Services
{
    public class ProdutoService : UrlBase,IProdutoService, IDisposable
    {
        private bool disposedValue;

        public async Task DeletarProdAsync(string keyProd)
        {
            string url = $"{_url}Product/DeleteOneProd/{keyProd}";
            using (HttpClient client = new())
            {
                client.BaseAddress = new Uri(url);
                await client.DeleteAsync(url);
            }
        }
        public async Task<ModelProduto?> RetornaProdutoPorID(string prodId)
        {
            string url = $"{_url}Product/ReturnProd/{prodId}";
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
                var Url = new Uri($"{_url}Product/GetProdSeller/{userId}");
                var response = await httpClient.GetAsync(Url);
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        return await response.Content.ReadFromJsonAsync<Dictionary<string, ModelProduto>>();
                    }
                    catch (Exception )
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
                var Url = new Uri($"{_url}Product/AlterProd/{userId}");
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
                var Url = new Uri($"{_url}Product/{userId}");
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
        public string PegarNomeUrl(string url)
        {
            string separar = url;
            string[] separar1 = separar.Split("%2F");
            string[] separar2 = separar1[3].Split("?");
            return separar2[0];
        }

        public async Task<Result> FinalizarCompra(Dictionary<string, ModelProduto> model)
        {
            float? valorTotal = 0;
            int? quantiaTotal = 0;
            List<dynamic> produtosEqtd = new();


            List<ModelProduto> prod = new();
            foreach (var idProduto in model)
            {
                var ID = idProduto.Value.IdProduto;
                ModelProduto item = await RetornaProdutoPorID(ID) ?? new ModelProduto { };
                valorTotal += item.PrecoProd * idProduto.Value.QtdPorProd;
                quantiaTotal += idProduto.Value.QtdPorProd;

                List<dynamic> unirLista = new()
                {
                    item,
                    idProduto.Value.QtdPorProd ?? 0,
                    idProduto.Key
                };
                produtosEqtd.Add(unirLista);

                // Implementar na model para gerar nota

                item.Qtd = idProduto.Value.QtdPorProd;
                item.ValorTotal = (float)Math.Round((decimal)((decimal)item.PrecoProd * idProduto.Value.QtdPorProd), 2);
                item.Cancelado = false;
                item.Data = DateTime.Now.ToString("dd/MM/yyyy");
                prod.Add(item);
            }
            Result rt = new()
            {
                Prod = prod,
                ProdutosEqtd = produtosEqtd,
                QuantiaTotal = quantiaTotal,
                ValorTotal = valorTotal,
            };
            return rt;


        }
        public async Task<Tuple<ModelProduto, List<dynamic>>> RetornoProdModel(ICarrinhoService _carrinho, string idUser ) {

            
            var model = new ModelProduto();
            List<dynamic> ProdutosEqtd = new();


            float? valorTotal = 0;
            int? quantiaTotal = 0;
            List<int?> qtdProdutos = new();

            List<ModelProduto> prod = new();
            var Produtos = await _carrinho.RetornaCarrinho(idUser);
            foreach (var idProduto in Produtos)
            {
                var ID = idProduto.Value.IdProduto;
                var qtdBase = (await RetornaProdutoPorID(ID)).Qtd;
                idProduto.Value.QtdPorProd = qtdBase <= idProduto.Value.QtdPorProd ? qtdBase : idProduto.Value.QtdPorProd;
                ModelProduto item = await RetornaProdutoPorID(ID) ?? new ModelProduto { };
                valorTotal += item.PrecoProd * idProduto.Value.QtdPorProd;
                quantiaTotal += idProduto.Value.QtdPorProd;

                List<dynamic> unirLista = new()
                {
                    item,
                    idProduto.Value.QtdPorProd,
                    idProduto.Key
                };

                ProdutosEqtd.Add(unirLista);
            }
            model.ValorTotal = Math.Round((double)valorTotal, 2);
            model.Qtd = quantiaTotal;


            return new Tuple<ModelProduto, List<dynamic>>(model, ProdutosEqtd);

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
    public class Result
    {
        public List<dynamic>? ProdutosEqtd;
        public float? ValorTotal;
        public int? QuantiaTotal;
        public List<ModelProduto>? Prod;
    }
}
