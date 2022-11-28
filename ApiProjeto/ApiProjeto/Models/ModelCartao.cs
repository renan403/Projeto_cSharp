namespace ApiMvc.Models
{
    public class ModelCartao
    {
        public string? NomeCard { get; set; }
        public string? NumeroCard { get; set; }
        public string? DataExpiracao { get; set; }
        public int? Cvv { get; set; }
        public string? Bandeira { get; set; }
        public string? Erro { get; set; } = string.Empty;
        public string? CaminhoImgBandeira { get; set; }
        public string? CaminhoImgCartao { get; set; }
        public bool Padrao { get; set; }
        public string? QuatroDigCard { get; set; }
    }
}
