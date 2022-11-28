using ApiMvc.FireData;
using ApiMvc.Repositories;

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
        public async Task<bool> RegisterUser(string? name, string? email)
        {
            string id = string.Empty;
            using Gerador gerador = new();
            id = gerador.AleatoriosUser();
            if (name == null || email == null)
            {
                return false;
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

                return true;
            }
            else
            {
                return false;
            }
        }
        public async Task<bool> LoginUser(string? email)
        {
            if (email != null)
            {

                var user = (await _client.Child("Usuarios").OnceAsync<ModelUsuario>())
                    .Where(u => u.Object.Email == email)
                    .FirstOrDefault();
                return (user != null);
            }
            return false;
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
        public async Task TrocarNome(string userId, string nome)
        {
            var key = await GetUserKey(userId);
            var info = await _client.Child($"Usuarios/{key}/InfoUser").OnceSingleAsync<ModelUsuario>();
            info.Nome = nome;
            await _client.Child($"Usuarios/{key}/InfoUser").PatchAsync(info);

        }
        public async Task DeletarConta(string userId, string email, string senha)
        {
            using (Auth auth = new())
            {
                using var prod = new ProductBase();
                var produtos = await prod.RetornarObjProdutosVendedorAsync(userId);
                try
                {
                    foreach (var i in produtos)
                    {
                        try
                        {
                            await auth.DeleteOneImage(userId, email, senha, FuncProduto.PegarNomeUrl(i.Object.UrlImg), i.Object.Path);
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
                    await auth.DeleteUser(email, senha);
                }
            }
        }
    }
}
