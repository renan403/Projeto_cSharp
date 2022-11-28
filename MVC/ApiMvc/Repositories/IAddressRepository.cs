using Firebase.Database;

namespace ApiMvc.Repositories
{
    public interface IAddressRepository
    {
        Task<FirebaseObject<ModelEndereco>?> RetornaEnderecoPadrao(string? idUser);
        Task<bool> SalvarEndereco(string userId, ModelEndereco model);
        Task<bool> AlterarEndereco(string? key, string userId, ModelEndereco objEnd);
        Task<bool> DeleteEndereco(string Key, string? userId);
        Task<IReadOnlyCollection<FirebaseObject<ModelEndereco>>?> PuxarEndereco(string? userId);
        Task<bool> VerificaPadrao(string idUser);
        Task<bool> DeleteUser(string email);
        Task<bool> MudarPadrao(string userId, string key);
    }
}
