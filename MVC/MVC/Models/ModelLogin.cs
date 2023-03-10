using System.ComponentModel.DataAnnotations;

namespace MVC.Models
{
    public class ModelLogin
    {     
        public string? Email { get; set; }
        public string? Senha { get; set; }
        public bool? Ativo { get; set; }
        public string? Nome { get; set; }
        public string? Resposta { get; set; }
        public int Exist { get; set; }
    }
}
