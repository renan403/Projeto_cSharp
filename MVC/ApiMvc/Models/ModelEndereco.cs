using System.ComponentModel.DataAnnotations;

namespace ApiMvc.Models
{
    public class ModelEndereco
    {
        [Required]
        public string? Pais { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Campo Login: aceita no máximo 100 caracteres.")]
        public string? Nome { get; set; }
        public string? Telefone { get; set; }
        [Required]
        [MinLength(9, ErrorMessage = "O campo {0} deve ter no mínimo {1} caracteres")]
        [MaxLength(9, ErrorMessage = "Cep Invalido")]
        public string? Cep { get; set; }
        [Required]
        public string? Endereco { get; set; }
        public string? Complemento { get; set; }
        [Required]
        public int Numero { get; set; }
        [Required]
        public string? Bairro { get; set; }
        [Required]
        public string? Cidade { get; set; }
        [Required]
        public string? Estado { get; set; }
        public bool Padrao { get; set; }
        public string? UF { get; set; }
    }
}
