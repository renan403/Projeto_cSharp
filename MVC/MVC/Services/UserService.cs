using Firebase.Auth;
using MVC.Services.Funcoes;
using System.Security.Policy;

namespace MVC.Services
{
    public class UserService : IDisposable
    {
        private bool disposedValue;
        private IConfiguration _iconfig;
        public UserService(IConfiguration iconfig)
        {
            _iconfig = iconfig;
        }
        public async Task<string?> RegistrarUser(string nome, string email, string pwd)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/Register/{nome}";
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
        public async Task<string?> RetornaID(string email)
        {
            try
            {
                var ch = Chave.GetKey();
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/ReturnId/{email}";
                using HttpClient client = new();
                var rt = await client.GetAsync(url);
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
        public async Task<string> RetornaNome(string userId)
        { 
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/ReturnName/{userId}";
                using HttpClient client = new();
                var rt = await client.GetAsync(url);
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
        public async Task<string> TrocarNome(string IdUser, string nome)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/ChangeName";
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
        public async Task<string?> Logar(string email, string pwd)
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/Login";
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
        public async Task<bool> DeletarConta(string idUser,string email, string pwd )
        {
            try
            {
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/DeleteAcount";
                using HttpClient client = new();
                var ch = Chave.GetKey();

                MultipartFormDataContent content = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "pwd"},
                    {new StringContent(idUser), "idUser" }
                };
                return Convert.ToBoolean(await (await client.PostAsync(url, content)).Content.ReadAsStringAsync());
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<string> ResetarSenha(string email)
        {
            try
            {
                using HttpClient client = new();
                string url = $"{_iconfig.GetValue<string>("UrlApi")}Auth/RecoverPassword";
                var ch = Chave.GetKey();
                MultipartFormDataContent content = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},                   
                };
                var rt = await client.PostAsync(url,content);
                if(rt.IsSuccessStatusCode)
                    return await rt.Content.ReadAsStringAsync();
                return "erro";
            }
            catch (Exception)
            {

                throw;
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
