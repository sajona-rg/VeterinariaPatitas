using System.ComponentModel.DataAnnotations;

namespace VeterinariaMVC.Models;

public class Especie
{
    public int Id { get; set; }

    [Display(Name = "Especie")]
    [Required, StringLength(50)]
    public string Nombre { get; set; } = string.Empty;

    public ICollection<Raza> Razas { get; set; } = new List<Raza>();
}
