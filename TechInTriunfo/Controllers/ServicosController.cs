
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Data;
using TechInTriunfo.Models;

public class ServicosController : Controller
{
    private readonly ApplicationDbContext _context;

    public ServicosController(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        return View(await _context.Servicos.ToListAsync());
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Servico servico)
    {
        if (ModelState.IsValid)
        {
            _context.Add(servico);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(servico);
    }


    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var servico = await _context.Servicos.FindAsync(id);
        if (servico == null) return NotFound();

        return View(servico);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Servico servico)
    {
        if (id != servico.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(servico);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServicoExists(servico.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(servico);
    }

  
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var servico = await _context.Servicos.FindAsync(id);
        if (servico == null) return NotFound();

        return View(servico);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var servico = await _context.Servicos.FindAsync(id);
        if (servico != null) _context.Servicos.Remove(servico);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    private bool ServicoExists(int id)
    {
        return _context.Servicos.Any(e => e.Id == id);
    }
}