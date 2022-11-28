using ApiMvc.FireData;
using ApiMvc.Repositories;
using Firebase.Database;

namespace ApiMvc.Database
{
    public class AddressBase : Base , IAddressRepository
    {       

        public async Task<FirebaseObject<ModelEndereco>?> RetornaEnderecoPadrao(string? idUser)
        {           
            var user = (await _client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.IdUser == idUser)
                .FirstOrDefault();
            if (user != null)
            {
                var enderecos = (await PuxarEndereco(idUser)).ToArray();
                foreach (var item in enderecos)
                {
                    if (item.Object.Padrao)
                    {
                        return item;
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
                await _client.Child($"Usuarios/{chave}/Endereco/{end.Key}").PatchAsync(new ModelEndereco()
                {
                    Nome = objEnd.Nome ?? end.Object.Nome,
                    Telefone = objEnd.Telefone ?? end.Object.Telefone,
                    Cep = objEnd.Cep ?? end.Object.Cep,
                    Endereco = objEnd.Endereco ?? end.Object.Endereco,
                    Pais = objEnd.Pais ?? end.Object.Pais,
                    Numero = objEnd.Numero == 0 ? end.Object.Numero : objEnd.Numero,
                    Complemento = objEnd.Complemento ?? end.Object.Complemento,
                    Bairro = objEnd.Bairro ?? end.Object.Bairro,
                    Cidade = objEnd.Cidade ?? end.Object.Cidade,
                    Estado = objEnd.Estado ?? end.Object.Estado,
                    Padrao = end.Object.Padrao,
                    UF = objEnd.UF ?? end.Object.UF
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
        public async Task<IReadOnlyCollection<FirebaseObject<ModelEndereco>>?> PuxarEndereco(string? userId)
        {
            if (userId != String.Empty)
            {
                var chave = await GetUserKey(userId);
                var enderecoResult = (await _client.Child($"Usuarios/{chave}/Endereco").OnceAsync<ModelEndereco>());

                return enderecoResult;
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
                    if (item.Object.Padrao)
                    {
                        return true;
                    }

                }
            }

            return false;
        }
        public async Task<bool> DeleteUser(string email)
        {
            try
            {
                var key = (await _client.Child("Usuarios").OnceAsync<ModelLogin>()).FirstOrDefault(m => m.Object.Email == email).Key;
                await _client.Child($"Usuarios/{key}").DeleteAsync();

                return true;
            }
            catch
            {
                return false;
            }

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

            var enderecos = ((await PuxarEndereco(userId)).Where(m => m.Object.Padrao == true)).FirstOrDefault();

            enderecos.Object.Padrao = false;
            await _client.Child($"Usuarios/{chave}/Endereco/{enderecos.Key}").PatchAsync(enderecos.Object);

            end.Object.Padrao = true;
            await _client.Child($"Usuarios/{chave}/Endereco/{end.Key}").PatchAsync(end.Object);


            return true;
        }
    }
}
