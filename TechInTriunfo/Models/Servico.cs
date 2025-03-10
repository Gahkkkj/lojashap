namespace TechInTriunfo.Models
{
    public class Servico
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty; 
        public decimal Valor { get; set; } 
        public string? Descricao { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.Now; 
    }
}