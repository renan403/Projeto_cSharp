using MVC.Models;

namespace MVC.Repository
{
    public interface IEnderecoService
    {
        Task<ModelEndereco?> RetornarEndPadrao(string? userId);
        Task SalvarEndereco(string userId, ModelEndereco m);
        Task DeletarEndereco(string userId, string opcao);
        Task<Dictionary<string, ModelEndereco>?> PuxarEnderecos(string userId);
        Task MudarPadrao(string userId, string key);
        Task AlterarEndereco(string userId, string key, ModelEndereco m);

    }
}
