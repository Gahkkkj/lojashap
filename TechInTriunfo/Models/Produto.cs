using System.ComponentModel.DataAnnotations;

namespace TechInTriunfo.Models
{
    public class Produto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Nome { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "Valor inválido")]
        public decimal Valor { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade inválida")]
        public int Quantidade { get; set; }

        public string? Descricao { get; set; }
        public byte[]? Imagem { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}