using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaMVC.Models;

public class Raza
{
    public int Id { get; set; }

    [Display(Name = "Raza")]
    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Especie")]
    public int IdEspecie { get; set; }

    [ForeignKey(nameof(IdEspecie))]
    public Especie? Especie { get; set; }

    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();
}
