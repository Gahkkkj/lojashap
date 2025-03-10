using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Data;
using TechInTriunfo.Models;


public class CarrinhoController : Controller
{
    private readonly ApplicationDbContext _context;

    public CarrinhoController(ApplicationDbContext context) => _context = context;

    private int? GetUsuarioId() => HttpContext.Session.GetInt32("UsuarioId");

    public async Task<IActionResult> Index()
    {
        var usuarioId = GetUsuarioId();
        if (usuarioId == null) return RedirectToAction("Login", "Usuarios");

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        return View(carrinho);
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int produtoId, int quantidade)
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuarios");

        var produto = await _context.Produtos.FindAsync(produtoId);
        if (produto == null) return NotFound();

        if (produto.Quantidade < quantidade)
        {
            TempData["Erro"] = "Quantidade indisponível em estoque";
            return RedirectToAction("Index", "Produtos");
        }

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId) 
            ?? new Carrinho { UsuarioId = usuarioId };

        var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        
        if (itemExistente != null)
        {
            itemExistente.Quantidade += quantidade;
        }
        else
        {
            carrinho.Itens.Add(new ItemCarrinho
            {
                ProdutoId = produtoId,
                Nome = produto.Nome,
                Valor = produto.Valor,
                Imagem = produto.Imagem,
                Quantidade = quantidade
            });
        }

        if (carrinho.Id == 0) _context.Carrinhos.Add(carrinho);
        
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int produtoId)
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuario");

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrinho == null) return NotFound();

        var item = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId);
        if (item != null)
        {
            carrinho.Itens.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Item removido do carrinho";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> FinalizarCompra()
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuarios");

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
            .ThenInclude(i => i.Produto)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrinho == null || !carrinho.Itens.Any())
        {
            TempData["Erro"] = "Carrinho vazio";
            return RedirectToAction(nameof(Index));
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            foreach (var item in carrinho.Itens)
            {
                var produto = await _context.Produtos.FindAsync(item.ProdutoId);
                if (produto == null) continue;

                if (produto.Quantidade < item.Quantidade)
                {
                    TempData["Erro"] = $"Estoque insuficiente para {produto.Nome}";
                    return RedirectToAction(nameof(Index));
                }

                produto.Quantidade -= item.Quantidade;
                if (produto.Quantidade <= 0)
                {
                    _context.Produtos.Remove(produto);
                }
            }

            _context.Carrinhos.Remove(carrinho);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("Index", "Produtos");
        }
        catch
        {
            await transaction.RollbackAsync();
            TempData["Erro"] = "Erro ao processar a compra";
            return RedirectToAction(nameof(Index));
        }
    }
    
}