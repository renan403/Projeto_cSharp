namespace ApiMvc.Models
{
    public class ModelNotaFiscal
    {
        public string? IdNota { get; set; }
        public DateTime? Registro { get; set; }
        public ModelDestinatario? Destinatario { get; set; }
        public ModelCartao? Cartao { get; set; }
        public List<ModelProduto>? Produto { get; set; }
        public ModelEndereco? Endereco { get; set; }
    }
}
