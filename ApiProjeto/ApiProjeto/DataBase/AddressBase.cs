using ApiMvc.FireData;
using ApiMvc.Repositories;
using Firebase.Database;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace ApiMvc.Database
{
    public class AddressBase : Base , IAddressRepository
    {       

        public async Task<ModelEndereco> RetornaEnderecoPadrao(string? idUser)
        {           
            var user = (await _client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.IdUser == idUser)
                .FirstOrDefault();
            if (user != null)
            {
                var enderecos = (await PuxarEndereco(idUser)).ToArray();
                foreach (var item in enderecos)
                {
                    if (item.Value.Padrao)
                    {
                        return item.Value;
                    }

                }
            }
            return null;
        }
        public async Task<bool> SalvarEndereco(string userId, ModelEndereco model)
        {
            if (userId == "" || model.Pais == "" || model.Nome == "" || model.Telefone == "" || model.Cep == "" || model.Endereco == "" || model.Numero == 0 || model.Bairro == "" || model.Cidade == "" || model.Estado == "" || model.UF == "")
            {
                return false;
            }
            var chave = await GetUserKey(userId);
            var count = (await _client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelUsuario>());
            var EnderecoConvert = count.ToArray();

            for (int i = 0; i <= count.Count; i++)
            {
                try
                {

                    if (!EnderecoConvert[i].Key.Contains($"End{i + 1}"))
                    {
                        var possuiPadrao = await VerificaPadrao(userId);
                        if (possuiPadrao)
                        {

                            await _client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                            if (model.Padrao == true)
                            {
                                await MudarPadrao(userId, $"End{i + 1}");
                            }
                        }
                        else
                        {
                            model.Padrao = true;
                            await _client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                        }
                        break;
                    }

                }
                catch
                {
                    var possuiPadrao = await VerificaPadrao(userId);
                    if (possuiPadrao)
                    {
                        await _client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                        if (model.Padrao == true)
                        {
                            await MudarPadrao(userId, $"End{i + 1}");
                        }

                    }
                    else
                    {
                        model.Padrao = true;
                        await _client.Child($"Usuarios/{chave}/Endereco/End{i + 1}").PatchAsync(model);
                    }


                }

            }
            return true;
        }
        public async Task<bool> AlterarEndereco(string? key, string userId, ModelEndereco objEnd)
        {
            try
            {
                if (key == null || userId == String.Empty)
                    return false;
                var chave = await GetUserKey(userId);
                var puxarEnd = await PuxarEndereco(userId);
                var end = puxarEnd?.Where(m => m.Key == key).FirstOrDefault();
                if (end == null)
                    return false;
                await _client.Child($"Usuarios/{chave}/Endereco/{end.Value.Key}").PatchAsync(new ModelEndereco()
                {
                    Nome = objEnd.Nome ?? end.Value.Value.Nome,
                    Telefone = objEnd.Telefone ?? end.Value.Value.Telefone,
                    Cep = objEnd.Cep ?? end.Value.Value.Cep,
                    Endereco = objEnd.Endereco ?? end.Value.Value.Endereco,
                    Pais = objEnd.Pais ?? end.Value.Value.Pais,
                    Numero = objEnd.Numero == 0 ? end.Value.Value.Numero : objEnd.Numero,
                    Complemento = objEnd.Complemento ?? end.Value.Value.Complemento,
                    Bairro = objEnd.Bairro ?? end.Value.Value.Bairro,
                    Cidade = objEnd.Cidade ?? end.Value.Value.Cidade,
                    Estado = objEnd.Estado ?? end.Value.Value.Estado,
                    Padrao = end.Value.Value.Padrao,
                    UF = objEnd.UF ?? end.Value.Value.UF
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public async Task<bool> DeleteEndereco(string Key, string? userId)
        {
            try
            {
                if (Key.Contains("End") && userId != null)
                {
                    var chave = await GetUserKey(userId);
                    await _client.Child($"Usuarios/{chave}/Endereco/{Key}").DeleteAsync();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
            
        }
        public async Task<Dictionary<string,ModelEndereco>?> PuxarEndereco(string? userId)
        {
            if (userId != String.Empty)
            {
                var chave = await GetUserKey(userId);
                var enderecoResult = (await _client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelEndereco>());

                return enderecoResult.ToDictionary(m => m.Key, m => m.Object);
            }
            else
            {
                return null;
            }


        }
        public async Task<bool> VerificaPadrao(string idUser)
        {
            if (idUser != null)
            {
                var enderecos = (await PuxarEndereco(idUser)).ToArray();
                foreach (var item in enderecos)
                {
                    if (item.Value.Padrao)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
      
        public async Task<bool> MudarPadrao(string userId, string key)
        {
            if (key == null || userId == String.Empty)
                return false;
            var chave = await GetUserKey(userId);
            var puxarEnd = await PuxarEndereco(userId);
            var end = puxarEnd?.Where(m => m.Key == key).FirstOrDefault();
            if (end == null)
                return false;

            var enderecos = ((await PuxarEndereco(userId)).Where(m => m.Value.Padrao == true)).FirstOrDefault();

            enderecos.Value.Padrao = false;
            await _client.Child($"Usuarios/{chave}/Endereco/{enderecos.Key}").PatchAsync(enderecos.Value);

            end.Value.Value.Padrao = true;
            await _client.Child($"Usuarios/{chave}/Endereco/{end.Value.Key}").PatchAsync(end.Value.Value);


            return true;
        }
    }
}
