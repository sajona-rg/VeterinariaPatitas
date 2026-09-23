using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using VeterinariaMVC.Models;

namespace VeterinariaMVC.ViewModels;


public class MascotasListadoViewModel
{
    public string? Texto { get; set; }
    public int? EspecieId { get; set; }
    public int? RazaId { get; set; }
    public int? PropietarioId { get; set; }
    public string? Orden { get; set; } = "nombre";

    [BindNever, ValidateNever]
    public SelectList Especies { get; set; } = default!;
    [BindNever, ValidateNever]
    public SelectList Razas { get; set; } = default!;
    [BindNever, ValidateNever]
    public SelectList Propietarios { get; set; } = default!;

    [BindNever, ValidateNever]
    public List<Mascota> Resultados { get; set; } = new();

    public bool HayFiltros =>
        !string.IsNullOrWhiteSpace(Texto) || EspecieId > 0 || RazaId > 0 || PropietarioId > 0;
}
