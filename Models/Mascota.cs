using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaMVC.Models;


public class Mascota
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "Escribe el nombre de la mascota.")]
    [StringLength(50, ErrorMessage = "Máximo {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Propietario")]
    [Required(ErrorMessage = "Elige el propietario.")]
    [Range(1, int.MaxValue, ErrorMessage = "Elige el propietario.")]
    public int IdPropietario { get; set; }

    [ForeignKey(nameof(IdPropietario))]
    public Propietario? Propietario { get; set; }

    [Display(Name = "Raza")]
    [Required(ErrorMessage = "Elige la raza.")]
    [Range(1, int.MaxValue, ErrorMessage = "Elige la raza.")]
    public int IdRaza { get; set; }

    [ForeignKey(nameof(IdRaza))]
    public Raza? Raza { get; set; }

    [Display(Name = "Fecha de nacimiento")]
    [DataType(DataType.Date)]
    [Column(TypeName = "date")]
    public DateTime? FechaNacimiento { get; set; }

    [Display(Name = "Peso (kg)")]
    [Range(typeof(decimal), "0.01", "999.99", ErrorMessage = "El peso debe estar entre {1} y {2} kg.")]
    [Column(TypeName = "decimal(5,2)")]
    public decimal? Peso { get; set; }

    [Display(Name = "Foto")]
    [StringLength(255)]
    public string? RutaFoto { get; set; }


    [NotMapped]
    public string? Edad
    {
        get
        {
            if (FechaNacimiento is null) return null;

            var hoy = DateTime.Today;
            var nacimiento = FechaNacimiento.Value.Date;
            if (nacimiento > hoy) return null;

            int meses = (hoy.Year - nacimiento.Year) * 12 + hoy.Month - nacimiento.Month;
            if (hoy.Day < nacimiento.Day) meses--;

            if (meses < 1) return "Menos de 1 mes";
            if (meses < 12) return meses == 1 ? "1 mes" : $"{meses} meses";

            int anios = meses / 12;
            return anios == 1 ? "1 año" : $"{anios} años";
        }
    }
}
