namespace MVC.Repository
{
    public interface IGeralService
    {
        string[] RetornoRNCUC(string endereco);
        string? RetornaNomeNull(string nome);
        string FormatarNomeNav(string nomeParaFormatar);
        string UltimosDigitos(string card);

    }
}
