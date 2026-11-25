using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

public class DuenoDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Dni { get; set; } = null!;
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public DateTime FechaRegistro { get; set; }
    public int CantidadMascotas { get; set; }
}

public class DuenoCrearDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = null!;

    [Required, RegularExpression(@"^\d{8}$", ErrorMessage = "El DNI debe tener 8 digitos.")]
    public string Dni { get; set; } = null!;

    [StringLength(15)]
    public string? Telefono { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }
}

public class DuenoActualizarDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string Nombre { get; set; } = null!;

    [StringLength(15)]
    public string? Telefono { get; set; }

    [EmailAddress, StringLength(100)]
    public string? Email { get; set; }
}
