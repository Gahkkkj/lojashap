using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace TechInTriunfo.Models
{
    public class Carrinho
    {
        public int Id { get; set; }

        // Solução 1: Tornar a propriedade anulável
        public Usuario? Usuario { get; set; }  // ← Corrigido com operador de nulidade

        // Solução 2: Manter como não anulável com inicialização forçada
        // public Usuario Usuario { get; set; } = null!;

        [Required]
        public int UsuarioId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public List<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();

        [NotMapped]
        public decimal Total => Itens.Sum(item => item.Valor * item.Quantidade);
    }
}