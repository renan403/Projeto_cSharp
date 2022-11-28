using MVC.Models.Service;

namespace MVC.Services
{
    public class UserService : IDisposable
    {
        private bool disposedValue;

        public async Task DeletarUser(string urlDeleteEmail)
        {
            using HttpClient client = new();
            await client.DeleteAsync(urlDeleteEmail);
        }
        public async Task<string?> RegistrarUser(string url, string email, string pwd)
        {
            try
            {
                using HttpClient client = new();
                var ch = Chave.GetKey();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "pwd"},
                };
                var rt = await client.PostAsync(url, form);
                if (rt.IsSuccessStatusCode)
                {
                    return await rt.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public async Task<string> RetornaID(string urlEmail)
        {
            try
            {
                using HttpClient client = new();
                var rt = await client.GetAsync(urlEmail);
                if (rt.IsSuccessStatusCode)
                {
                    return await rt.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception e)
            {

                return null;
            }
            
        }
        public async Task<string> RetornaNome(string urlIdUser)
        {
            try
            {
                using HttpClient client = new();
                var rt = await client.GetAsync(urlIdUser);
                if (rt.IsSuccessStatusCode)
                {
                    return await rt.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception e)
            {

                return null;
            }

        }
        public async Task<string> TrocarNome(string url, string IdUser, string nome)
        {
            try
            {
                using HttpClient client = new();
                MultipartFormDataContent form = new() 
                {
                    {new StringContent(IdUser), "idUser"},
                    {new StringContent(nome), "nome"}
                };
                var rt = await client.PatchAsync(url, form);
                if (rt.IsSuccessStatusCode)
                {
                    return "alterado com sucesso";
                }
                return "erro na alteração";
            }
            catch (Exception e)
            {
                return "erro";
            }

        }
        public async Task<string?> Logar(string url, string email, string pwd)
        {
            try
            {
                using HttpClient client = new();
                var ch = Chave.GetKey();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "pwd"}
                };
                var rt = await client.PostAsync(url, form);
                if (rt.IsSuccessStatusCode)
                {
                    return await rt.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        public async Task<bool> DeletarConta(string urlDelConta, string email, string pwd, string idUser)
        {
            try
            {
                using HttpClient client = new();
                var ch = Chave.GetKey();

                MultipartFormDataContent content = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "pwd"},
                    {new StringContent(idUser), "idUser" }
                };
                return Convert.ToBoolean(await (await client.PostAsync(urlDelConta, content)).Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return false;
            }
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
