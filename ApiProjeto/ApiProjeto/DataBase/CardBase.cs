using ApiMvc.FireData;
using ApiMvc.Repositories;
using Firebase.Database;

namespace ApiMvc.Database
{
    public class CardBase : Base ,ICardRepository
    {
 

        public async Task<string> SalvarCard(string userId, ModelCartao model)
        {
            string cvvstring = model.Cvv.ToString() ?? "";
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
                var count = await _client.Child($"Usuarios/{key}/Card").OnceAsync<ModelProduto>();
                var cartoes = count.ToArray();
                for (int i = 0; i <= cartoes.Length; i++)
                {
                    try
                    {
                        if (!cartoes[i].Key.Contains($"Cartao{i + 1}"))
                        {
                            if (await VerificaCardPadrao(userId))
                            {
                                await _client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                                return "sucesso";
                            }
                            else
                            {
                                model.Padrao = true;
                                await _client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                                return "sucesso";
                            }
                        }

                    }
                    catch
                    {
                        if (await VerificaCardPadrao(userId))
                        {
                            await _client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
                            return "sucesso";
                        }
                        else
                        {
                            model.Padrao = true;
                            await _client.Child($"Usuarios/{key}/Card/Cartao{i + 1}").PatchAsync(model);
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
            return (await _client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).ToArray();

        }
        public async Task DeleteCard(string userId, string cartao)
        {
            var key = await GetUserKey(userId);

            await _client.Child($"Usuarios/{key}/Card/{cartao}").DeleteAsync();
            if (await VerificaCardPadrao(userId) is false)
            {
                var cart = (await _client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).FirstOrDefault();
                if (cart != null)
                {
                    cart.Object.Padrao = true;
                    await _client.Child($"Usuarios/{key}/Card/{cart.Key}").PatchAsync(cart.Object);
                }
            }
        }
        public async Task<bool> AlterarCard(string userId, string cartao, string nome, string data)
        {
            var key = await GetUserKey(userId);
            var dadosCard = (await _client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>()).Where(m => m.Key == cartao).FirstOrDefault();
            if (dadosCard == null)
                return false;
            await _client.Child($"Usuarios/{key}/Card/{cartao}").PatchAsync(new ModelCartao()
            {
                NomeCard = nome,
                Cvv = dadosCard.Object.Cvv,
                CaminhoImgBandeira = dadosCard.Object.CaminhoImgBandeira,
                NumeroCard = dadosCard.Object.NumeroCard,
                Bandeira = dadosCard.Object.Bandeira,
                DataExpiracao = data,
                Erro = dadosCard.Object.Erro,
                QuatroDigCard = dadosCard.Object.QuatroDigCard
            });
            return true;
        }
        public async Task<ModelCartao> Cartao(string userId, string keyCard)
        {
            var key = await GetUserKey(userId);
            return (await _client.Child($"Usuarios/{key}/Card/{keyCard}").OnceSingleAsync<ModelCartao>());
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

            var cartoes = (await _client.Child($"Usuarios/{key}/Card").OnceAsync<ModelCartao>());
            foreach (var card in cartoes)
            {
                if (card.Object.Padrao)
                    return card;
            }

            return null;
        }
        public async Task<bool> MudarPadraoCard(string userId, string key)
        {
            var chave = await GetUserKey(userId);
            var cartao = await Cartao(userId, key);
            try
            {
                var CardAtualPadrao = await RetornarCartaoPadrao(userId);
                CardAtualPadrao.Object.Padrao = false;
                await _client.Child($"Usuarios/{chave}/Card/{CardAtualPadrao.Key}").PatchAsync(CardAtualPadrao.Object);
                cartao.Padrao = true;
                await _client.Child($"Usuarios/{chave}/Card/{key}").PatchAsync(cartao);
                return true;
            }
            catch
            {
                return false;
            }

        }
    }
}
