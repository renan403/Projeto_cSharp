using ApiMvc.FireData;
using ApiMvc.Repositories;
using ApiProjeto.Functions;

namespace ApiMvc.Database
{
    public class UserBase : Base ,IUserRepository
    {


        public async Task<bool> IsUserExists(string email)
        {
            var user = (await _client.Child("Usuarios")
                .OnceAsync<ModelUsuario>())
                .Where(u => u.Object.Email == email)
                .FirstOrDefault();
            return (user != null);
        }
        public async Task<string> RegisterUser(string name, string email, string pwd)
        {
            using Auth auth = new();
            email = Criptografia.Decriptografar(Convert.FromBase64String(email));
            pwd = Criptografia.Decriptografar(Convert.FromBase64String(pwd));
           
            var rt = await auth.RegisterEmail(email, pwd, name);
            if (rt == "Criado")
            {
                string id = string.Empty;
                using Gerador gerador = new();
                id = gerador.AleatoriosUser();
                if (name == null || email == null)
                {
                    return "nome ou email vazio";
                }
                if (await IsUserExists(email) == false)
                {
                    await _client.Child("Usuarios")
                        .PostAsync(new ModelUsuario()
                        {
                            IdUser = id,
                            Email = email
                        });
                    var key = await GetUserKey(id);
                    await _client.Child($"Usuarios/{key}/InfoUser").PatchAsync(new ModelUsuario
                    {
                        Nome = name,
                    });

                    return rt;
                }
                else
                {
                    return "entre em contato com suporte";
                }

            }
            return rt;
            
        }
        public async Task<string> LoginUser(string email, string pwd)
        {
            Auth auth = new();
            email = Criptografia.Decriptografar(Convert.FromBase64String(email));
            pwd = Criptografia.Decriptografar(Convert.FromBase64String(pwd));
            if (email != null)
            {
                var autorizado = await auth.LoginEmail(email, pwd);
                if (autorizado == "Logged")
                {
                    var user = (await _client.Child("Usuarios").OnceAsync<ModelUsuario>())
                   .Where(u => u.Object.Email == email)
                   .FirstOrDefault();
                    if (user != null)
                        return autorizado;
                    if (user == null)
                        return "Entre em contato com suporte";
                }
                else
                    return autorizado;
                
            }
            return "erro";
        }
        public async Task<string> RetornaNome(string? idUser)
        {

            var user = (await _client.Child("Usuarios").OnceAsync<ModelUsuario>())
                .Where(u => u.Object.IdUser == idUser)
                .FirstOrDefault();
            if (user != null)
            {
                var key = await GetUserKey(idUser);
                var nome = (await _client.Child($"Usuarios/{key}/InfoUser").OnceSingleAsync<ModelUsuario>());
                return nome.Nome ?? "";
            }


            return string.Empty;
        }
        public async Task<string> RetornaID(string? email)
        {
            try
            {
                var user = (await _client.Child($"Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.Email;
                if (user != null)
                {
                    var id = (await _client.Child("Usuarios").OnceAsync<ModelUsuario>()).Where(u => u.Object.Email == email).FirstOrDefault().Object.IdUser;
                    return id ?? "";
                }
            }
            catch (Exception)
            {
                throw;
            }
            return string.Empty;
        }
        public async Task<bool> TrocarNome(string userId, string nome)
        {
            try
            {
                var key = await GetUserKey(userId);
                var info = await _client.Child($"Usuarios/{key}/InfoUser").OnceSingleAsync<ModelUsuario>();
                info.Nome = nome;
                await _client.Child($"Usuarios/{key}/InfoUser").PatchAsync(info);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
            

        }
        public async Task<bool> DeletarConta(string userId, string email, string pwd)
        {           
            try
            {
                using (Auth auth = new())
                {
                    email = Criptografia.Decriptografar(Convert.FromBase64String(email));
                    pwd = Criptografia.Decriptografar(Convert.FromBase64String(pwd));
                    using var prod = new ProductBase();
                    var produtos = await prod.RetornarObjProdutosVendedorAsync(userId);
                    try
                    {
                        foreach (var i in produtos)
                        {
                            try
                            {
                                await auth.DeleteOneImage(userId, email, pwd, FuncProduto.PegarNomeUrl(i.Value.UrlImg), i.Value.Path);
                                await prod.DeleteUmProdutoAsync(i.Key);
                            }
                            catch
                            {
                                continue;
                            }
                        }
                    }
                    finally
                    {
                        await auth.DeleteUser(email, pwd);                     
                    }
                    return true;
                }
            }
            catch (Exception)
            {

                return false;
            }
           
        }
        public async Task<bool> DeleteUser(string email)
        {
            try
            {
                var key = (await _client.Child("Usuarios").OnceAsync<ModelLogin>()).Where(m => m.Object.Email == email).FirstOrDefault().Key;
                await _client.Child($"Usuarios/{key}").DeleteAsync();

                return true;
            }
            catch
            {
                return false;
            }

        }
        public async Task<string> ResetarSenha(string email)
        {
            Auth auth = new();
            return await auth.ResetPassword(email);
        }
    }
}
