using Firebase.Database;

namespace ApiMvc.Repositories
{
    public interface IAddressRepository
    {
        Task<ModelEndereco> RetornaEnderecoPadrao(string? idUser);
        Task<bool> SalvarEndereco(string userId, ModelEndereco model);
        Task<bool> AlterarEndereco(string? key, string userId, ModelEndereco objEnd);
        Task<bool> DeleteEndereco(string Key, string? userId);
        Task<Dictionary<string, ModelEndereco>?> PuxarEndereco(string? userId);
        Task<bool> VerificaPadrao(string idUser);
        Task<bool> MudarPadrao(string userId, string key);
    }
}
