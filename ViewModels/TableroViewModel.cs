using VeterinariaMVC.Models;

namespace VeterinariaMVC.ViewModels;

public class TableroViewModel
{
    public int TotalPropietarios { get; set; }
    public int TotalMascotas { get; set; }
    public int TotalEspecies { get; set; }
    public int TotalRazas { get; set; }

    public List<ConteoEspecie> PorEspecie { get; set; } = new();

    public List<Mascota> Recientes { get; set; } = new();
}

public record ConteoEspecie(string Especie, int Cantidad);
