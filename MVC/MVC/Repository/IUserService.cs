using MVC.Models;
using MVC.Models.User;

namespace MVC.Repository
{
    public interface IUserService
    {

        Task<Logar?> Logar(string? email, string? senha);
        Task<string?> RegistrarUser(string? nome, string? email, string? pwd);
        Task<string?> RetornaID(string? email);
        Task<string> TrocarNome(string? IdUser, string? nome);
        Task<bool> DeletarConta(string? idUser, string? email, string? pwd);
        Task<string> ResetarSenha(string? email);
        Task<string?> RetornaNome(string? userId);
    }
}
