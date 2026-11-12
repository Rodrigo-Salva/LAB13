using System.ComponentModel.DataAnnotations;

namespace Veterinaria.Entidades.Dtos;

public class MascotaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = null!;
    public string Especie { get; set; } = null!;
    public string? Raza { get; set; }
    public int Edad { get; set; }
    public decimal Peso { get; set; }
    public bool Activo { get; set; }
    public int DuenoId { get; set; }
    public string DuenoNombre { get; set; } = null!;
}

public class MascotaCrearDto
{
    [Required, StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(30)]
    public string Especie { get; set; } = null!;

    [StringLength(50)]
    public string? Raza { get; set; }

    [Range(0, 40)]
    public int Edad { get; set; }

    [Range(0.01, 200)]
    public decimal Peso { get; set; }

    [Range(1, int.MaxValue)]
    public int DuenoId { get; set; }
}

public class MascotaActualizarDto
{
    [Required, StringLength(50)]
    public string Nombre { get; set; } = null!;

    [Required, StringLength(30)]
    public string Especie { get; set; } = null!;

    [StringLength(50)]
    public string? Raza { get; set; }

    [Range(0, 40)]
    public int Edad { get; set; }

    [Range(0.01, 200)]
    public decimal Peso { get; set; }

    public bool Activo { get; set; } = true;
}
