using System;
using System.ComponentModel.DataAnnotations;

namespace TechInTriunfo.Models
{
    public class Contato
    {
        public int Id { get; set; } 

        [Required(ErrorMessage = "Nome é obrigatório")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mensagem é obrigatória")]
        public string Mensagem { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}