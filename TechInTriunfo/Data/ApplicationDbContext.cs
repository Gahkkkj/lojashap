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
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<PedidoItem> PedidoItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        
            modelBuilder.Entity<ItemCarrinho>()
                .ToTable("itens_carrinho"); 

            modelBuilder.Entity<PedidoItem>(entity =>
            {
                entity.ToTable("pedidoitems"); 
                
                entity.Property(e => e.Tipo)
                    .HasConversion<string>()
                    .HasColumnType("enum('Produto','Servico')"); 

                entity.HasOne(pi => pi.Produto)
                    .WithMany()
                    .HasForeignKey(pi => pi.ProdutoId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(pi => pi.Servico)
                    .WithMany()
                    .HasForeignKey(pi => pi.ServicoId)
                    .OnDelete(DeleteBehavior.SetNull);
            });


            modelBuilder.Entity<Carrinho>(entity =>
            {
                entity.HasOne(c => c.Usuario)
                    .WithOne()
                    .HasForeignKey<Carrinho>(c => c.UsuarioId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<ItemCarrinho>(entity =>
            {
                entity.HasOne(ic => ic.Carrinho)
                    .WithMany(c => c.Itens)
                    .HasForeignKey(ic => ic.CarrinhoId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}