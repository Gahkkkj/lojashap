using System;
using System.Collections.Generic;
using System.Linq;

namespace TechInTriunfo.Models
{
    public class Carrinho
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public List<ItemCarrinho> Itens { get; set; } = new List<ItemCarrinho>();

        public decimal Total => Itens.Sum(item => item.Valor * item.Quantidade);
    }
}