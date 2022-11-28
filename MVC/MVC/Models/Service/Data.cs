using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Firebase.Auth;
using MVC.Services;

//Fazer DLL do Banco
//Fazer encapsulamento

namespace MVC.Models.Service
{
    public class Data : IDisposable
    {
        readonly FirebaseClient client;

        private bool disposedValue;

        public Data()
        {
            client = new FirebaseClient("https://projetoport-50b66-default-rtdb.firebaseio.com/");

        }
        private async Task<string> GetUserKey(string userId)
        {
            return (await client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(m => m.Object.IdUser == userId).FirstOrDefault().Key;
        }

        public async Task<Array> RetornarNotaFiscal(string userId)
        {
            var key = await GetUserKey(userId);
            var notas = await client.Child($"Usuarios/{key}/NotaFiscal").OnceAsync<ModelNotaFiscal>();
            return notas.OrderByDescending(m => m.Object.Registro).ToArray();
        }
        public async Task<bool> GerarNotaFiscal(string userId,ModelNotaFiscal notaFiscal)
        {
            float? total = 0;
            Gerador gera = new();
            for (int i = 0; i < notaFiscal.Produto.Count; i++)
            {
                notaFiscal.Produto[i].Data = DateTime.Now.ToString("dd 'de' MMMM 'de' yyyy");
                notaFiscal.Produto[i].Enviado = true;
                notaFiscal.Produto[i].Recebido = false;
                total += notaFiscal.Produto[i].ValorTotal;
            }
            notaFiscal.Produto[0].ValorTotal = (float?)Math.Round((decimal)total, 2);
            notaFiscal.Registro = DateTime.Now;
            string numPedido = $"{ gera.AleatoriosComprarProd() }-{ gera.AleatoriosComprarProd()}";
            var key = await GetUserKey(userId);
            await client.Child($"Usuarios/{key}/NotaFiscal/{numPedido}").PatchAsync(notaFiscal);
            return true;
        }
        public async Task<ModelNotaFiscal> PegarNotaFiscal(string userId, string nota)
        {
            var key = await GetUserKey(userId);

            return await client.Child($"Usuarios/{key}/NotaFiscal/{nota}").OnceSingleAsync<ModelNotaFiscal>();
        }
        public async Task ConfirmarPedido(string userId, string nota, string nome)
        {
            var key = await GetUserKey(userId);
            var produtos = await PegarNotaFiscal(userId, nota);
            ModelDestinatario destinatario = new ModelDestinatario {Nome= nome , Data = DateTime.Now.ToString("dd 'de' MMMMM 'de' yyyy") }; 
            foreach (var produto in produtos.Produto)
            {
                produtos.Destinatario = destinatario;
                produto.Recebido = true;
            }
            await client.Child($"Usuarios/{key}/NotaFiscal/{nota}").PatchAsync(produtos);
        }  

        //----------------------------------------------------------------------------------------------------------------------------------------------------------------
        //               Cartão
        public async Task<string> SalvarCard(string userId, ModelCartao model)
        {
            string cvvstring = model.Cvv.ToString();
            if (model.Bandeira == "Amex" && cvvstring.Length < 4)
            {
                return "⚠️ Cartão não adicionado, CVV Faltando 1 número.";
            }
            else if (model.Bandeira != "Amex" && cvvstring.Length > 3)
            {
                return "⚠️ Cartão não adicionado, CVV está com 1 número a mais.";
            }
            else
            {
                model.Padrao = false;
                model.QuatroDigCard = model.NumeroCard.Substring(model.NumeroCard.Length - 4);
                var key = await GetUserKey(userId);
                var count = await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelProduto>();
                var cartoes = count.ToArray();
                for (int i = 0; i <= cartoes.Length; i++)
                {
                    try
                    {
                        if (!cartoes[i].Key.Contains($"Cartao{i + 1}"))
                        {
                            if (await VerificaCardPadrao(userId))
                            {
                                await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                                return "sucesso";
                            }
                            else
                            {
                                model.Padrao = true;
                                await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                                return "sucesso";
                            }
                        }

                    }
                    catch
                    {
                        if (await VerificaCardPadrao(userId))
                        {
                            await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                            return "sucesso";
                        }
                        else
                        {
                            model.Padrao = true;
                            await client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                            return "sucesso";
                        }

                    }

                }
            }
            return "falha";
        }
        public async Task<IReadOnlyCollection<FirebaseObject<ModelCartao>>> ReturnCard(string userId)
        {
            var key = await GetUserKey(userId);
            return (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).ToArray(); 
            
        }
        public async Task DeleteCard(string userId, string cartao)
        {
          var key = await GetUserKey(userId);
            
            await client.Child($"Usuarios/{key}/Card/{cartao}").DeleteAsync();
            if(await VerificaCardPadrao(userId) is false)
            {
                var cart = (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).FirstOrDefault();
                if(cart != null)
                {
                    cart.Object.Padrao = true;
                    await client.Child($"Usuarios/{key}/Card/{cart.Key}").PatchAsync(cart.Object);
                }
            }
        }
        public async Task<bool> AlterarCard(string userId, string cartao, string nome, string data)
        {
            var key = await GetUserKey(userId);
            var dadosCard = (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).Where(m => m.Key == cartao).FirstOrDefault();
            if(dadosCard == null)
                return false;
            await client.Child($"Usuarios/{key}/Card/{cartao}").PatchAsync(new ModelCartao()
            {
                NomeCard = nome,
                Cvv = dadosCard.Object.Cvv,
                CaminhoImgBandeira = dadosCard.Object.CaminhoImgBandeira,
                NumeroCard = dadosCard.Object.NumeroCard,
                Bandeira = dadosCard.Object.Bandeira,
                DataExpiracao = data,
                Erro = dadosCard.Object.Erro,
                QuatroDigCard = dadosCard.Object.QuatroDigCard
            }) ;
            return true;
        }
        public async Task<ModelCartao> Cartao(string userId, string keyCard)
        {
            var key = await GetUserKey(userId);
            return (await client.Child($"Usuarios/{key}/Card/{keyCard}").OnceSingleAsync<ModelCartao>());            
        }
        public async Task<bool> VerificaCardPadrao(string userId)
        {
            if (userId != null)
            {
                var cartoes = (await ReturnCard(userId)).ToArray();
                foreach (var cartao in cartoes)
                {
                    if (cartao.Object.Padrao)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        public async Task<FirebaseObject<ModelCartao>?> RetornarCartaoPadrao(string userId)
        {
            var key = await GetUserKey(userId);

            var cartoes = (await client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>());
            foreach(var card in cartoes)
            {
                if (card.Object.Padrao)
                    return card;
            }
           
            return null;
        }
        public async Task<bool> MudarPadraoCard(string userId,string key)
        {
            var chave = await GetUserKey(userId);
            var cartao = await Cartao(userId, key);
            try
            {
                var CardAtualPadrao = await RetornarCartaoPadrao(userId);
                CardAtualPadrao.Object.Padrao = false;
                await client.Child($"Usuarios/{chave}/Card/{CardAtualPadrao.Key}").PatchAsync(CardAtualPadrao.Object);
                cartao.Padrao = true;
                await client.Child($"Usuarios/{chave}/Card/{key}").PatchAsync(cartao);
                return true;
            }
            catch
            {
                return false;
            }
           
        }     

        //------------------------------------------------------------------------------------------------------------------------------------------------------------------
        //                  Carrinho

        
        public async Task<bool> SalvarProdCarrinho(string userId, string idprod , int quantidade)
        {
            var KeyUser = await GetUserKey(userId);
            var count = await client.Child($"Usuarios/{KeyUser}/Carrinho").OnceAsync<ModelProduto>();
            var carrinho = count.ToArray();

            for (int i = 0; i <= carrinho.Length; i++)
            {
                try
                {
                    if (!carrinho[i].Key.Contains($"Produto{i + 1}"))
                    {
                        await client.Child($"Usuarios/{KeyUser}/Carrinho/Produto{i + 1}").PatchAsync(new ModelProduto { IdProduto = idprod , QtdPorProd = quantidade});
                        return true;
                    }

                }
                catch
                {
                    await client.Child($"Usuarios/{KeyUser}/Carrinho/Produto{i + 1}").PatchAsync(new ModelProduto { IdProduto = idprod, QtdPorProd = quantidade });
                    return true;
                }

            }
            return false;
        }
        public async Task<bool> DeleteProdCarrinho(string Key, string userId)
        {
            if (Key.Contains("Produto") && userId != null)
            {
                var chave = await GetUserKey(userId);
                await client.Child($"Usuarios/{chave}/Carrinho/{Key}").DeleteAsync();
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public async Task<bool> AlterarQtdCarrinho(string userId, string idProd, int quantidade)
        {
            CarrinhoService carrinhoService = new CarrinhoService();
            var carrinho = await carrinhoService.RetornaCarrinho(userId);

            foreach(var car in carrinho)
            {
                if(car.Value.IdProduto == idProd)
                {
                    var key = await GetUserKey(userId);
                    await client.Child($"Usuarios/{key}/Carrinho/{car.Key}").PatchAsync(new ModelProduto
                    {
                        IdProduto = car.Value.IdProduto,
                        QtdPorProd = quantidade
                    });
                    return true;
                }
            }
            return false;
        }
        public async Task<string> AddQtdCarrinho(string userId, string idProd, int quantidade)
        {
            CarrinhoService carr = new();
            var carrinho = await carr.RetornaCarrinho(userId);

            foreach (var car in carrinho)
            {
                if (car.Value.IdProduto == idProd)
                {
                    if(car.Value.QtdPorProd + quantidade > 5)
                    {
                        return $"{car.Value.QtdPorProd}";
                    }
                    var key = await GetUserKey(userId);
                    await client.Child($"Usuarios/{key}/Carrinho/{car.Key}").PatchAsync(new ModelProduto
                    {
                        IdProduto = car.Value.IdProduto,
                        QtdPorProd = car.Value.QtdPorProd + quantidade
                    });
                    return "sucesso";
                }
            }
            return "erro";
        }
        public async Task<bool> PossuiNoCarrinho(string idClient, string idProd)
        {
            CarrinhoService carr = new();

            var carrinho = await carr.RetornaCarrinho(idClient);

            foreach (var car in carrinho)
            {
                if (car.Value.IdProduto == idProd)
                {
                    return true;
                }
            }
            return false;
        }        
        public async Task DeletarCarrinho(string userId)
        {
            var key = await GetUserKey(userId);
            await client.Child($"Usuarios/{key}/Carrinho").DeleteAsync();
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


