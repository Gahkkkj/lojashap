using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TechInTriunfo.Data;
using TechInTriunfo.Models;
using Microsoft.AspNetCore.Identity;

public class UsuariosController : Controller
{
    private readonly ApplicationDbContext _context;

    public UsuariosController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var usuarioId = HttpContext.Session.GetInt32("UsuarioId");
        if (usuarioId == null) return RedirectToAction("Login");

        var usuarios = await _context.Usuarios.ToListAsync();
        return View(usuarios);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null) return NotFound();

        var usuario = await _context.Usuarios.FindAsync(id);
        return usuario != null ? View(usuario) : NotFound();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nome,Email,Telefone")] Usuario usuario)
    {
        if (id != usuario.Id) return NotFound();

        if (ModelState.IsValid)
        {
            try
            {
              
                var existingUser = await _context.Usuarios
                    .FirstOrDefaultAsync(u => u.Email == usuario.Email && u.Id != id);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "Este email já está em uso");
                    return View(usuario);
                }

                var userToUpdate = await _context.Usuarios.FindAsync(id);
                if (userToUpdate == null) return NotFound(); 
                userToUpdate.Nome = usuario.Nome;
                userToUpdate.Email = usuario.Email;
                userToUpdate.Telefone = usuario.Telefone;

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.Id)) return NotFound();
                throw;
            }
            return RedirectToAction(nameof(Index));
        }
        return View(usuario);
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null) return NotFound();

        var usuario = await _context.Usuarios
            .FirstOrDefaultAsync(m => m.Id == id);

        return usuario != null ? View(usuario) : NotFound();
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            var usuario = await _context.Usuarios
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var carrinho = await _context.Carrinhos
                .Include(c => c.Itens)
                .FirstOrDefaultAsync(c => c.UsuarioId == id);

            if (carrinho != null)
            {
                _context.ItensCarrinho.RemoveRange(carrinho.Itens);
                _context.Carrinhos.Remove(carrinho);
                await _context.SaveChangesAsync(); 
            }

            
            _context.Usuarios.Remove(new Usuario { Id = id }); 
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await UsuarioExistsAsync(id))
            {
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError("", "Erro ao excluir. Tente novamente.");
                return await Delete(id);
            }
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<bool> UsuarioExistsAsync(int id)
    {
        return await _context.Usuarios.AnyAsync(e => e.Id == id);
    }

    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(Usuario usuario, string ConfirmarSenha)
    {
        if (usuario.Senha != ConfirmarSenha)
            ModelState.AddModelError("ConfirmarSenha", "Senhas não coincidem");

        if (ModelState.IsValid)
        {
            if (_context.Usuarios.Any(u => u.Email == usuario.Email))
            {
                ModelState.AddModelError("Email", "Email já cadastrado");
                return View(usuario);
            }

            var passwordHasher = new PasswordHasher<Usuario>();
            usuario.Senha = passwordHasher.HashPassword(usuario, usuario.Senha);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }
        return View(usuario);
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Login(string email, string senha)
    {
        var usuario = _context.Usuarios.FirstOrDefault(u => u.Email == email);
        if (usuario != null)
        {
            var passwordHasher = new PasswordHasher<Usuario>();
            if (passwordHasher.VerifyHashedPassword(usuario, usuario.Senha, senha) == PasswordVerificationResult.Success)
            {
                HttpContext.Session.SetInt32("UsuarioId", usuario.Id);
                HttpContext.Session.SetString("UsuarioNome", usuario.Nome);
                return RedirectToAction("Index", "Produtos");
            }
        }
        ModelState.AddModelError("", "Credenciais inválidas");
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Remove("UsuarioId");
        HttpContext.Session.Remove("UsuarioNome");
        return RedirectToAction("Index", "Produtos");
    }

    private bool UsuarioExists(int id)
    {
        return _context.Usuarios.Any(e => e.Id == id);
    }
}