using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.Controllers;

public class PropietariosController : Controller
{
    private readonly DataContext _db;

    public PropietariosController(DataContext db) => _db = db;

    public async Task<IActionResult> Index(string? texto)
    {
        var consulta = _db.Propietarios
            .AsNoTracking()
            .Include(p => p.Mascotas)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(texto))
        {
            var t = texto.Trim();
            consulta = consulta.Where(p =>
                p.Nombre.Contains(t) ||
                p.Apellido.Contains(t) ||
                (p.Email != null && p.Email.Contains(t)) ||
                (p.Telefono != null && p.Telefono.Contains(t)));
        }

        ViewData["Texto"] = texto;
        var propietarios = await consulta
            .OrderBy(p => p.Apellido).ThenBy(p => p.Nombre)
            .ToListAsync();

        return View(propietarios);
    }

    public async Task<IActionResult> Ficha(int id)
    {
        var propietario = await _db.Propietarios
            .AsNoTracking()
            .Include(p => p.Mascotas).ThenInclude(m => m.Raza).ThenInclude(r => r!.Especie)
            .FirstOrDefaultAsync(p => p.Id == id);

        return propietario is null ? NotFound() : View(propietario);
    }

    public IActionResult Nuevo() => View(new Propietario());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nuevo(Propietario propietario)
    {
        if (!ModelState.IsValid)
            return View(propietario);

        _db.Propietarios.Add(propietario);
        await _db.SaveChangesAsync();

        TempData["Aviso"] = $"{propietario.NombreCompleto} quedó registrado.";
        return RedirectToAction(nameof(Ficha), new { id = propietario.Id });
    }

    public async Task<IActionResult> Editar(int id)
    {
        var propietario = await _db.Propietarios.FindAsync(id);
        return propietario is null ? NotFound() : View(propietario);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Propietario propietario)
    {
        if (id != propietario.Id)
            return BadRequest();

        if (!ModelState.IsValid)
            return View(propietario);

        try
        {
            _db.Propietarios.Update(propietario);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!_db.Propietarios.Any(p => p.Id == id))
        {
            return NotFound();
        }

        TempData["Aviso"] = "Los datos del propietario se actualizaron.";
        return RedirectToAction(nameof(Ficha), new { id });
    }

    public async Task<IActionResult> Eliminar(int id)
    {
        var propietario = await _db.Propietarios
            .AsNoTracking()
            .Include(p => p.Mascotas)
            .FirstOrDefaultAsync(p => p.Id == id);

        return propietario is null ? NotFound() : View(propietario);
    }

    [HttpPost, ActionName(nameof(Eliminar))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var propietario = await _db.Propietarios
            .Include(p => p.Mascotas)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (propietario is null)
            return RedirectToAction(nameof(Index));

        if (propietario.Mascotas.Count > 0)
        {
            TempData["Alerta"] = $"{propietario.NombreCompleto} tiene {propietario.Mascotas.Count} mascota(s) a su nombre. Reasígnalas o elimínalas antes.";
            return RedirectToAction(nameof(Ficha), new { id });
        }

        _db.Propietarios.Remove(propietario);
        await _db.SaveChangesAsync();

        TempData["Aviso"] = "Propietario eliminado.";
        return RedirectToAction(nameof(Index));
    }
}
