using System;
using System.Collections.Generic;

namespace Veterinaria.Entidades.Models;

public partial class Mascota
{
    public int Id { get; set; }

    public string Nombre { get; set; } = null!;

    public string Especie { get; set; } = null!;

    public string? Raza { get; set; }

    public int Edad { get; set; }

    public decimal Peso { get; set; }

    public bool Activo { get; set; }

    public int DuenoId { get; set; }

    public virtual Dueno Dueno { get; set; } = null!;
}
