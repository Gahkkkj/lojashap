namespace TechInTriunfo.Models
{
    public class ItemCarrinho
    {
        public int Id { get; set; }
        public int CarrinhoId { get; set; }
        public int? ProdutoId { get; set; }
        public int? ServicoId { get; set; }
        public int Quantidade { get; set; }

        public required Carrinho Carrinho { get; set; } 
        public Produto? Produto { get; set; }
        public Servico? Servico { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public byte[]? Imagem { get; set; }
    }
    
}

