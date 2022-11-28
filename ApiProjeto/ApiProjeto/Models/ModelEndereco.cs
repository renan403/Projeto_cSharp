﻿using System.ComponentModel.DataAnnotations;

namespace ApiMvc.Models
{
    public class ModelEndereco
    {   
        public string? Pais { get; set; } 
        public string? Nome { get; set; } 
        public string? Telefone { get; set; } 
        public string? Cep { get; set; }
        public string? Endereco { get; set; } 
        public string? Complemento { get; set; }   
        public int Numero { get; set; }     
        public string? Bairro { get; set; } 
        public string? Cidade { get; set; } 
        public string? Estado { get; set; }
        public bool Padrao { get; set; }
        public string? UF { get; set; }
    }
}
