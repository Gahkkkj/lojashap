using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Data;
using TechInTriunfo.Models;

public class PedidosController : Controller
{
    private readonly ApplicationDbContext _context;

    public PedidosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Itens)
            .ThenInclude(i => i.Produto)
            .Include(p => p.Usuario) 
            .ToListAsync();

        return View(pedidos);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var pedido = await _context.Pedidos
            .Include(p => p.Usuario) 
            .FirstOrDefaultAsync(p => p.Id == id);

        if (pedido == null) return NotFound();

        return View(pedido);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var pedido = await _context.Pedidos.FindAsync(id);
        if (pedido != null)
        {
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private bool PedidoExists(int id)
    {
        return _context.Pedidos.Any(e => e.Id == id);
    }
}