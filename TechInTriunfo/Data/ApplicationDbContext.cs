using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Models;

namespace TechInTriunfo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Carrinho> Carrinhos { get; set; }
        public DbSet<Contato> Contatos { get; set; } 

        public DbSet<ItemCarrinho> ItensCarrinho { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ItemCarrinho>()
                .ToTable("itens_carrinho");
        }
    }
}