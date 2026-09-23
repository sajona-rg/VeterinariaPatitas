using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VeterinariaMVC.Models;


public class Propietario
{
    public int Id { get; set; }

    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "Escribe el nombre.")]
    [StringLength(50, ErrorMessage = "Máximo {1} caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Display(Name = "Apellido")]
    [Required(ErrorMessage = "Escribe el apellido.")]
    [StringLength(50, ErrorMessage = "Máximo {1} caracteres.")]
    public string Apellido { get; set; } = string.Empty;

    [Display(Name = "Teléfono")]
    [StringLength(20, ErrorMessage = "Máximo {1} caracteres.")]
    [Phone(ErrorMessage = "El teléfono no tiene un formato válido.")]
    public string? Telefono { get; set; }

    [Display(Name = "Correo electrónico")]
    [StringLength(100, ErrorMessage = "Máximo {1} caracteres.")]
    [EmailAddress(ErrorMessage = "El correo no tiene un formato válido.")]
    public string? Email { get; set; }

    [Display(Name = "Dirección")]
    [StringLength(150, ErrorMessage = "Máximo {1} caracteres.")]
    public string? Direccion { get; set; }

    public ICollection<Mascota> Mascotas { get; set; } = new List<Mascota>();

    [NotMapped]
    public string NombreCompleto => $"{Nombre} {Apellido}".Trim();

    [NotMapped]
    public string Iniciales =>
        $"{(Nombre.Length > 0 ? Nombre[0] : ' ')}{(Apellido.Length > 0 ? Apellido[0] : ' ')}".Trim().ToUpperInvariant();
}
