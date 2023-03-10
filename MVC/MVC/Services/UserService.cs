using Firebase.Auth;
using MVC.Funcoes;
using MVC.Models.User;
using MVC.Repository;
using System.Security.Permissions;
using System.Security.Policy;

namespace MVC.Services
{
    public class UserService : UrlBase, IDisposable, IUserService
    {
        private bool disposedValue;
        

        public async Task<string?> RegistrarUser(string? nome, string? email, string? pwd)
        {
            try
            {
                if (string.IsNullOrEmpty(nome) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd))
                    return null;

                string url = $"{_url}Auth/Register/{nome}";
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
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string?> RetornaID(string? email)
        {
            try
            {
                if (email == null)
                    return null;
                var ch = Chave.GetKey();
                string url = $"{_url}Auth/ReturnId/{email}";
                using HttpClient client = new();
                var rt = await client.GetAsync(url);
                if (rt.IsSuccessStatusCode)
                {
                    return await rt.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string?> RetornaNome(string? userId)
        {
            try
            {
                if (userId == null)
                    return null;
                string url = $"{_url}Auth/ReturnName/{userId}";
                using HttpClient client = new();
                var rt = await client.GetAsync(url);
                if (rt.IsSuccessStatusCode)
                {
                    return await rt.Content.ReadAsStringAsync();
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }
        public async Task<string> TrocarNome(string? IdUser, string? nome)
        {
            try
            {
                if (IdUser == null || nome == null)
                    return "erro";
                string url = $"{_url}Auth/ChangeName";
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
            catch (Exception)
            {
                return "erro";
            }
        }
        public async Task<Logar?> Logar(string? email, string? pwd)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return new Logar();
                string url = $"{_url}Auth/Login";
                using HttpClient client = new();
                var ch = Chave.GetKey();
                MultipartFormDataContent form = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(pwd, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "pwd"}
                };
                var rt = await client.PostAsync(url, form);
                return await rt.Content.ReadAsStringAsync() switch
                {
                    "Logged" => new Logar() { Resposta = "Logged" },
                    "TOO_MANY_ATTEMPTS_TRY_LATER" => new Logar()
                    {
                        Resposta = "TOO_MANY_ATTEMPTS_TRY_LATER",
                        Erro = "Acesso desabilitado",
                        MessageErro = "Muitas falhas de login, para entrar imediatamente você pode trocar a senha ou pode tentar novamente mais tarde.",
                        SrcErro = "../icones/exclamation-triangle.svg"
                    },
                    "EMAIL_NOT_FOUND" => new Logar()
                    {
                        Resposta = "EMAIL_NOT_FOUND",
                        Erro = "Email",
                        MessageErro = "Email não encontrado, verifique por favor.",
                        SrcErro = "../icones/exclamation-triangle.svg"
                    },
                    "INVALID_PASSWORD" => new Logar()
                    {
                        Resposta = "INVALID_PASSWORD",
                        Erro = "Senha",
                        MessageErro = "Senha incorreta, verifique por favor.",
                        SrcErro = "../icones/exclamation-triangle.svg"
                    },
                    _ => new Logar()
                    {
                        Resposta = "SYSTEM_ERROR",
                        Erro = "Sistema",
                        MessageErro = "Sistema está fora do ar, por favor, tente mais tarde.",
                        SrcErro = "../icones/exclamation-triangle.svg"
                    },
                };
            }
            catch (Exception)
            {
                return new Logar()
                {
                    Resposta = "SYSTEM_ERROR",
                    Erro = "Sistema",
                    MessageErro = "Sistema está fora do ar, por favor, tente mais tarde.",
                    SrcErro = "../icones/exclamation-triangle.svg"
                };
            }
        }
        public async Task<bool> DeletarConta(string? idUser, string? email, string? pwd)
        {
            try
            {
                if (string.IsNullOrEmpty(idUser) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(pwd))
                    return false;
                string url = $"{_url}Auth/DeleteAcount";
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
        public async Task<string> ResetarSenha(string? email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                    return "erro";
                using HttpClient client = new();
                string url = $"{_url}Auth/RecoverPassword";
                var ch = Chave.GetKey();
                MultipartFormDataContent content = new()
                {
                    {new StringContent(Convert.ToBase64String(Seguranca.Cript.EncryptarStringParaByte(email, ch.Item1, ch.Item2, "#P>EET|MkkPa{oE0[Zcm"))), "email"},
                };
                var rt = await client.PostAsync(url, content);
                if (rt.IsSuccessStatusCode)
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
