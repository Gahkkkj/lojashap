using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Data;
using TechInTriunfo.Models;

public class ProdutosController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProdutosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string query)
    {
        var produtosQuery = _context.Produtos.AsQueryable();

        if (!string.IsNullOrEmpty(query))
        {
            produtosQuery = produtosQuery.Where(p => p.Nome.Contains(query));
        }

        var produtos = await produtosQuery.ToListAsync();
        return View(produtos);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Produto produto, IFormFile Imagem)
    {
        if (ModelState.IsValid)
        {
            if (Imagem != null && Imagem.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await Imagem.CopyToAsync(memoryStream);
                    produto.Imagem = memoryStream.ToArray();
                }
            }

            _context.Add(produto);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(produto);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        return View(produto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Produto produto, IFormFile Imagem)
    {
        if (id != produto.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {

                var produtoExistente = await _context.Produtos.FindAsync(id);
                
                produtoExistente.Nome = produto.Nome;
                produtoExistente.Valor = produto.Valor;
                produtoExistente.Quantidade = produto.Quantidade;
                produtoExistente.Descricao = produto.Descricao;

                if (Imagem != null && Imagem.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await Imagem.CopyToAsync(memoryStream);
                        produtoExistente.Imagem = memoryStream.ToArray();
                    }
                }

                _context.Update(produtoExistente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(produto.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(produto);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null) return NotFound();

        return View(produto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        _context.Produtos.Remove(produto);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public IActionResult GetImage(int id)
    {
        var produto = _context.Produtos.Find(id);
        if (produto?.Imagem != null)
        {
            return File(produto.Imagem, "image/jpeg");
        }
        return NotFound();
    }

    private bool ProdutoExists(int id)
    {
        return _context.Produtos.Any(e => e.Id == id);
    }
}