using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VeterinariaMVC.Data;
using VeterinariaMVC.Models;
using VeterinariaMVC.ViewModels;

namespace VeterinariaMVC.Controllers;

public class TableroController : Controller
{
    private readonly DataContext _db;

    public TableroController(DataContext db) => _db = db;

    public async Task<IActionResult> Index()
    {
        var modelo = new TableroViewModel
        {
            TotalPropietarios = await _db.Propietarios.CountAsync(),
            TotalMascotas = await _db.Mascotas.CountAsync(),
            TotalEspecies = await _db.Especies.CountAsync(),
            TotalRazas = await _db.Razas.CountAsync(),

            PorEspecie = await _db.Especies
                .AsNoTracking()
                .OrderByDescending(e => e.Razas.SelectMany(r => r.Mascotas).Count())
                .ThenBy(e => e.Nombre)
                .Select(e => new ConteoEspecie(e.Nombre, e.Razas.SelectMany(r => r.Mascotas).Count()))
                .ToListAsync(),

            Recientes = await _db.Mascotas
                .AsNoTracking()
                .Include(m => m.Propietario)
                .Include(m => m.Raza).ThenInclude(r => r!.Especie)
                .OrderByDescending(m => m.Id)
                .Take(5)
                .ToListAsync()
        };

        return View(modelo);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error() =>
        View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
}
