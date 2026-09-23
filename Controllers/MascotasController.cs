using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.ViewModels;

namespace VeterinariaMVC.Controllers;

public class MascotasController : Controller
{
    private const long TamanoMaximoFoto = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] ExtensionesPermitidas = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };

    private readonly DataContext _db;
    private readonly IWebHostEnvironment _entorno;

    public MascotasController(DataContext db, IWebHostEnvironment entorno)
    {
        _db = db;
        _entorno = entorno;
    }

    // GET: /Mascotas?texto=&especieId=&razaId=&propietarioId=&orden=
    public async Task<IActionResult> Index(MascotasListadoViewModel filtro)
    {
        // JOIN Mascotas + Propietarios + Razas + Especies
        var consulta = _db.Mascotas
            .AsNoTracking()
            .Include(m => m.Propietario)
            .Include(m => m.Raza).ThenInclude(r => r!.Especie)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro.Texto))
        {
            var t = filtro.Texto.Trim();
            consulta = consulta.Where(m =>
                m.Nombre.Contains(t) ||
                m.Propietario!.Nombre.Contains(t) ||
                m.Propietario!.Apellido.Contains(t) ||
                m.Raza!.Nombre.Contains(t) ||
                m.Raza!.Especie!.Nombre.Contains(t));
        }

        if (filtro.EspecieId > 0)
            consulta = consulta.Where(m => m.Raza!.IdEspecie == filtro.EspecieId);

        if (filtro.RazaId > 0)
            consulta = consulta.Where(m => m.IdRaza == filtro.RazaId);

        if (filtro.PropietarioId > 0)
            consulta = consulta.Where(m => m.IdPropietario == filtro.PropietarioId);

        consulta = filtro.Orden switch
        {
            "recientes" => consulta.OrderByDescending(m => m.Id),
            "peso"      => consulta.OrderByDescending(m => m.Peso),
            "edad"      => consulta.OrderBy(m => m.FechaNacimiento == null).ThenBy(m => m.FechaNacimiento),
            _           => consulta.OrderBy(m => m.Nombre)
        };

        filtro.Resultados = await consulta.ToListAsync();

        filtro.Especies = new SelectList(
            await _db.Especies.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
            nameof(Especie.Id), nameof(Especie.Nombre), filtro.EspecieId);

        // El selector de raza del filtro también depende de la especie
        var razas = _db.Razas.AsNoTracking().AsQueryable();
        if (filtro.EspecieId > 0)
            razas = razas.Where(r => r.IdEspecie == filtro.EspecieId);
        filtro.Razas = new SelectList(
            await razas.OrderBy(r => r.Nombre).ToListAsync(),
            nameof(Raza.Id), nameof(Raza.Nombre), filtro.RazaId);

        filtro.Propietarios = new SelectList(
            await _db.Propietarios.AsNoTracking().OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToListAsync(),
            nameof(Propietario.Id), nameof(Propietario.NombreCompleto), filtro.PropietarioId);

        return View(filtro);
    }

    // GET: /Mascotas/Ficha/5
    public async Task<IActionResult> Ficha(int id)
    {
        var mascota = await _db.Mascotas
            .AsNoTracking()
            .Include(m => m.Propietario)
            .Include(m => m.Raza).ThenInclude(r => r!.Especie)
            .FirstOrDefaultAsync(m => m.Id == id);

        return mascota is null ? NotFound() : View(mascota);
    }

    // GET: /Mascotas/Nuevo?propietarioId=3
    public async Task<IActionResult> Nuevo(int? propietarioId)
    {
        var mascota = new Mascota { IdPropietario = propietarioId ?? 0 };
        await CargarSelectoresAsync(mascota);
        return View(mascota);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Nuevo(Mascota mascota, IFormFile? foto)
    {
        mascota.RutaFoto = null; // la ruta solo la asigna el servidor
        ValidarFoto(foto);
        ValidarFechaNacimiento(mascota);

        if (!ModelState.IsValid)
        {
            await CargarSelectoresAsync(mascota);
            return View(mascota);
        }

        if (foto is { Length: > 0 })
            mascota.RutaFoto = await GuardarFotoAsync(foto);

        _db.Mascotas.Add(mascota);
        await _db.SaveChangesAsync();

        TempData["Aviso"] = $"{mascota.Nombre} ya es paciente de la clínica.";
        return RedirectToAction(nameof(Ficha), new { id = mascota.Id });
    }

    // GET: /Mascotas/Editar/5
    public async Task<IActionResult> Editar(int id)
    {
        var mascota = await _db.Mascotas.FindAsync(id);
        if (mascota is null)
            return NotFound();

        await CargarSelectoresAsync(mascota);
        return View(mascota);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Editar(int id, Mascota mascota, IFormFile? foto, bool quitarFoto = false)
    {
        if (id != mascota.Id)
            return BadRequest();

        // La ruta de la foto se toma de la BD, no del formulario
        var fotoActual = await _db.Mascotas
            .Where(m => m.Id == id)
            .Select(m => m.RutaFoto)
            .FirstOrDefaultAsync();
        mascota.RutaFoto = fotoActual;

        ValidarFoto(foto);
        ValidarFechaNacimiento(mascota);

        if (!ModelState.IsValid)
        {
            await CargarSelectoresAsync(mascota);
            return View(mascota);
        }

        if (foto is { Length: > 0 })
        {
            mascota.RutaFoto = await GuardarFotoAsync(foto);
            EliminarArchivoFoto(fotoActual);
        }
        else if (quitarFoto)
        {
            mascota.RutaFoto = null;
            EliminarArchivoFoto(fotoActual);
        }

        try
        {
            _db.Mascotas.Update(mascota);
            await _db.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException) when (!_db.Mascotas.Any(m => m.Id == id))
        {
            return NotFound();
        }

        TempData["Aviso"] = $"Se guardaron los cambios de {mascota.Nombre}.";
        return RedirectToAction(nameof(Ficha), new { id });
    }

    // GET: /Mascotas/Eliminar/5
    public async Task<IActionResult> Eliminar(int id)
    {
        var mascota = await _db.Mascotas
            .AsNoTracking()
            .Include(m => m.Propietario)
            .Include(m => m.Raza).ThenInclude(r => r!.Especie)
            .FirstOrDefaultAsync(m => m.Id == id);

        return mascota is null ? NotFound() : View(mascota);
    }

    [HttpPost, ActionName(nameof(Eliminar))]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EliminarConfirmado(int id)
    {
        var mascota = await _db.Mascotas.FindAsync(id);
        if (mascota is not null)
        {
            _db.Mascotas.Remove(mascota);
            await _db.SaveChangesAsync();
            EliminarArchivoFoto(mascota.RutaFoto);
            TempData["Aviso"] = $"{mascota.Nombre} fue retirada del registro.";
        }
        return RedirectToAction(nameof(Index));
    }

    // GET: /Mascotas/RazasDeEspecie/2  → JSON para los selectores dependientes
    [HttpGet]
    public async Task<IActionResult> RazasDeEspecie(int id)
    {
        var razas = await _db.Razas
            .AsNoTracking()
            .Where(r => r.IdEspecie == id)
            .OrderBy(r => r.Nombre)
            .Select(r => new { id = r.Id, nombre = r.Nombre })
            .ToListAsync();

        return Json(razas);
    }

    // ---------------------------------------------------------------- apoyo

    /// <summary>Llena propietarios, especies y las razas de la especie de la mascota.</summary>
    private async Task CargarSelectoresAsync(Mascota mascota)
    {
        int? especieId = mascota.IdRaza > 0
            ? await _db.Razas.Where(r => r.Id == mascota.IdRaza).Select(r => (int?)r.IdEspecie).FirstOrDefaultAsync()
            : null;

        ViewBag.EspecieId = especieId;

        ViewBag.Propietarios = new SelectList(
            await _db.Propietarios.AsNoTracking().OrderBy(p => p.Apellido).ThenBy(p => p.Nombre).ToListAsync(),
            nameof(Propietario.Id), nameof(Propietario.NombreCompleto), mascota.IdPropietario);

        ViewBag.Especies = new SelectList(
            await _db.Especies.AsNoTracking().OrderBy(e => e.Nombre).ToListAsync(),
            nameof(Especie.Id), nameof(Especie.Nombre), especieId);

        var razas = especieId is null
            ? new List<Raza>()
            : await _db.Razas.AsNoTracking().Where(r => r.IdEspecie == especieId).OrderBy(r => r.Nombre).ToListAsync();
        ViewBag.Razas = new SelectList(razas, nameof(Raza.Id), nameof(Raza.Nombre), mascota.IdRaza);
    }

    private void ValidarFoto(IFormFile? foto)
    {
        if (foto is null || foto.Length == 0)
            return;

        var extension = Path.GetExtension(foto.FileName).ToLowerInvariant();
        if (!ExtensionesPermitidas.Contains(extension))
            ModelState.AddModelError("foto", "Formato no admitido. Usa JPG, PNG, WEBP o GIF.");
        else if (foto.Length > TamanoMaximoFoto)
            ModelState.AddModelError("foto", "La foto supera los 5 MB.");
    }

    private void ValidarFechaNacimiento(Mascota mascota)
    {
        if (mascota.FechaNacimiento > DateTime.Today)
            ModelState.AddModelError(nameof(Mascota.FechaNacimiento), "La fecha de nacimiento no puede ser futura.");
    }

    /// <summary>Guarda la imagen en wwwroot/images y devuelve la ruta relativa para la BD.</summary>
    private async Task<string> GuardarFotoAsync(IFormFile foto)
    {
        var carpeta = Path.Combine(_entorno.WebRootPath, "images");
        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"{Guid.NewGuid():N}{Path.GetExtension(foto.FileName).ToLowerInvariant()}";
        await using var destino = new FileStream(Path.Combine(carpeta, nombreArchivo), FileMode.Create);
        await foto.CopyToAsync(destino);

        return $"/images/{nombreArchivo}";
    }

    private void EliminarArchivoFoto(string? rutaFoto)
    {
        if (string.IsNullOrWhiteSpace(rutaFoto) || !rutaFoto.StartsWith("/images/"))
            return;

        var archivo = Path.Combine(_entorno.WebRootPath, "images", Path.GetFileName(rutaFoto));
        if (System.IO.File.Exists(archivo))
            System.IO.File.Delete(archivo);
    }
}
