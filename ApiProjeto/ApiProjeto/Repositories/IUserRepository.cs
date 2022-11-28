namespace ApiMvc.Repositories
{
    public interface IUserRepository
    {     
        Task<string> RegisterUser(string name, string email, string pwd);
        Task<string> LoginUser(string email, string pwd);
        Task<string> RetornaNome(string? idUser);
        Task<string> RetornaID(string? email);
        Task<bool> TrocarNome(string userId, string nome);
        Task<bool> DeletarConta(string userId, string email, string senha);
    }
}
