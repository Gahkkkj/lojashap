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
            .Include(c => c.Itens)
                .ThenInclude(i => i.Servico)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        return View(carrinho ?? new Carrinho());
    }

    [HttpPost]
    public async Task<IActionResult> AddToCart(int? produtoId, int? servicoId, int quantidade = 1)
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuarios");

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId)
            ?? new Carrinho { UsuarioId = usuarioId };

        if (produtoId.HasValue)
        {
            var produto = await _context.Produtos.FindAsync(produtoId.Value);
            if (produto == null) return NotFound();

            if (produto.Quantidade < quantidade)
            {
                TempData["Erro"] = "Quantidade indisponível em estoque";
                return RedirectToAction("Index", "Produtos");
            }

            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ProdutoId == produtoId.Value);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += quantidade;
            }
            else
            {
                carrinho.Itens.Add(new ItemCarrinho
                {
                    Carrinho = carrinho, 
                    ProdutoId = produtoId.Value,
                    Nome = produto.Nome,
                    Valor = produto.Valor,
                    Imagem = produto.Imagem ?? Array.Empty<byte>(),
                    Quantidade = quantidade
                });

            }
        }
        else if (servicoId.HasValue)
        {
            var servico = await _context.Servicos.FindAsync(servicoId.Value);
            if (servico == null) return NotFound();

            var itemExistente = carrinho.Itens.FirstOrDefault(i => i.ServicoId == servicoId.Value);

            if (itemExistente != null)
            {
                itemExistente.Quantidade += 1;
            }
            else
            {
                carrinho.Itens.Add(new ItemCarrinho
                {
                    Carrinho = carrinho, 
                    ServicoId = servicoId.Value,
                    Nome = servico.Nome,
                    Valor = servico.Valor,
                    Quantidade = 1
                });
            }
        }

        if (carrinho.Id == 0) _context.Carrinhos.Add(carrinho);

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Remove(int itemId)
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuarios");

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrinho == null) return NotFound();

        var item = carrinho.Itens.FirstOrDefault(i => i.Id == itemId);
        if (item != null)
        {
            carrinho.Itens.Remove(item);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Item removido do carrinho";
        }

        return RedirectToAction(nameof(Index));
    }

     [HttpPost]
    public async Task<IActionResult> UpdateQuantidade(int itemId, int newQuantity)
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuarios");

        var item = await _context.ItensCarrinho
            .Include(i => i.Produto)
            .FirstOrDefaultAsync(i => i.Id == itemId);

        if (item == null) return NotFound();

        if (newQuantity < 1)
        {
            return await Remove(itemId);
        }

        if (item.Produto != null)
        {
            var produto = await _context.Produtos.FindAsync(item.ProdutoId);
            if (produto == null)
            {
                TempData["Erro"] = "Produto não encontrado";
                return RedirectToAction(nameof(Index));
            }

            if (produto.Quantidade < newQuantity)
            {
                TempData["Erro"] = $"Estoque insuficiente para {item.Nome}. Disponível: {produto.Quantidade}";
                return RedirectToAction(nameof(Index));
            }
        }

        item.Quantidade = newQuantity;
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> AumentaQuantidade(int itemId)
    {
        var item = await _context.ItensCarrinho.FindAsync(itemId);
        if (item == null) return NotFound();

        int newQuantity = item.Quantidade + 1;
        return await UpdateQuantidade(itemId, newQuantity);
    }

    [HttpPost]
    public async Task<IActionResult> DiminuiQuantidade(int itemId)
    {
        var item = await _context.ItensCarrinho.FindAsync(itemId);
        if (item == null) return NotFound();

        int newQuantity = item.Quantidade - 1;
        return await UpdateQuantidade(itemId, newQuantity);
    }
    [HttpPost]
    public async Task<IActionResult> FinalizarCompra()
    {
        var usuarioId = GetUsuarioId() ?? 0;
        if (usuarioId == 0) return RedirectToAction("Login", "Usuarios");

        var carrinho = await _context.Carrinhos
            .Include(c => c.Itens)
                .ThenInclude(i => i.Produto)
            .Include(c => c.Itens)
                .ThenInclude(i => i.Servico)
            .FirstOrDefaultAsync(c => c.UsuarioId == usuarioId);

        if (carrinho == null || !carrinho.Itens.Any())
        {
            TempData["Erro"] = "Carrinho vazio";
            return RedirectToAction(nameof(Index));
        }

        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null)
            {
                TempData["Erro"] = "Usuário não encontrado";
                return RedirectToAction(nameof(Index));
            }

            var pedido = new Pedido
            {
                UsuarioId = usuarioId,
                Usuario = usuario,
                Total = carrinho.Itens.Sum(i => i.Valor * i.Quantidade),
                DataPedido = DateTime.Now
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            var pedidoItems = carrinho.Itens.Select(item => new PedidoItem
            {
                PedidoId = pedido.Id,
                Nome = item.Nome,
                ValorUnitario = item.Valor,
                Quantidade = item.Quantidade,
                Tipo = item.ProdutoId != null ? TipoItem.Produto : TipoItem.Servico,
                ProdutoId = item.ProdutoId,
                ServicoId = item.ServicoId
            }).ToList();

            await _context.PedidoItems.AddRangeAsync(pedidoItems);

            var produtosParaAtualizar = carrinho.Itens
                .Where(i => i.ProdutoId != null)
                .Select(i => i.ProdutoId!.Value)
                .Distinct()
                .ToList();

            var produtos = await _context.Produtos
                .Where(p => produtosParaAtualizar.Contains(p.Id))
                .ToListAsync();

            foreach (var produto in produtos)
            {
                var quantidadeComprada = carrinho.Itens
                    .Where(i => i.ProdutoId == produto.Id)
                    .Sum(i => i.Quantidade);

                produto.Quantidade -= quantidadeComprada;
            }

            var produtosZerados = produtos.Where(p => p.Quantidade <= 0).ToList();
            if (produtosZerados.Any())
            {
                _context.Produtos.RemoveRange(produtosZerados);
            }

            _context.Carrinhos.Remove(carrinho);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return RedirectToAction("PedidoConfirmado", new { id = pedido.Id });
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            TempData["Erro"] = $"Erro ao processar a compra: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    public async Task<IActionResult> PedidoConfirmado(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Usuario)
            .Include(p => p.Itens)
                .ThenInclude(pi => pi.Produto)
            .Include(p => p.Itens)
                .ThenInclude(pi => pi.Servico)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null)
        {
            TempData["Erro"] = "Pedido não encontrado";
            return RedirectToAction(nameof(Index));
        }

        return View(pedido);
    }
}