using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Data;
using TechInTriunfo.Models;

public class ContatoController : Controller
{
    private readonly ApplicationDbContext _context;

    public ContatoController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var contatos = await _context.Contatos.ToListAsync();
        return View(contatos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Contato contato)
    {
        if (ModelState.IsValid)
        {
            _context.Add(contato);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Mensagem enviada com sucesso!";
            return RedirectToAction(nameof(Create));
        }
        return View(contato);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var contato = await _context.Contatos.FindAsync(id);
        if (contato == null) return NotFound();

        return View(contato);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Contato contato)
    {
        if (id != contato.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(contato);
                await _context.SaveChangesAsync();
                TempData["Sucesso"] = "Contato atualizado com sucesso!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContatoExists(contato.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(contato);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var contato = await _context.Contatos
            .FirstOrDefaultAsync(m => m.Id == id);
        if (contato == null) return NotFound();

        return View(contato);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var contato = await _context.Contatos.FindAsync(id);
        if (contato != null)
        {
            _context.Contatos.Remove(contato);
            await _context.SaveChangesAsync();
            TempData["Sucesso"] = "Contato excluído com sucesso!";
        }
        return RedirectToAction(nameof(Index));
    }

    private bool ContatoExists(int id)
    {
        return _context.Contatos.Any(e => e.Id == id);
    }
}