using System.ComponentModel.DataAnnotations.Schema;

namespace TechInTriunfo.Models
{
    public class PedidoItem
    {
        public int Id { get; set; }
        public int PedidoId { get; set; }
        public required string Nome { get; set; } = string.Empty; 

        [Column(TypeName = "decimal(10,2)")]
        public decimal ValorUnitario { get; set; }

        public int Quantidade { get; set; }
        public TipoItem Tipo { get; set; }

        public Produto? Produto { get; set; }
        public Servico? Servico { get; set; }

        public int? ProdutoId { get; set; }
        public int? ServicoId { get; set; }
    }
}