using System;
using System.ComponentModel.DataAnnotations;

namespace TechInTriunfo.Models
{
    public class Contato
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório")]

        public string Nome { get; set; } = string.Empty; 

        [Required(ErrorMessage = "E-mail é obrigatório")]
        [EmailAddress(ErrorMessage = "E-mail inválido")]
        public string Email { get; set; } = string.Empty; 

        [Required(ErrorMessage = "Mensagem é obrigatória")]
        public string Mensagem { get; set; } = string.Empty; 

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}