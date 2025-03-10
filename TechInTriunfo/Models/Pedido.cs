using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechInTriunfo.Models
{
    public class Pedido
    {
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        public required Usuario Usuario { get; set; } 

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Total { get; set; }

        [Required]
        public DateTime DataPedido { get; set; } = DateTime.Now;

        public List<PedidoItem> Itens { get; set; } = new List<PedidoItem>();
    }
}