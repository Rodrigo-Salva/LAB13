using ServicioVacunas.Entidades.Models;

namespace ServicioVacunas.Datos.Repositorios;

public interface IVacunaRepositorio
{
    Task<List<Vacuna>> ListarAsync();
    Task<Vacuna?> ObtenerPorIdAsync(int id);
}

// Datos en memoria (este servicio no tiene base de datos)
public class VacunaRepositorio : IVacunaRepositorio
{
    private static readonly List<Vacuna> Vacunas = new()
    {
        new(1, "Antirrábica", "Perro", "Anual", 35.00m),
        new(2, "Parvovirus", "Perro", "Anual", 45.00m),
        new(3, "Triple felina", "Gato", "Anual", 50.00m),
        new(4, "Leucemia felina", "Gato", "Anual", 60.00m)
    };

    public Task<List<Vacuna>> ListarAsync() => Task.FromResult(Vacunas.OrderBy(v => v.Id).ToList());

    public Task<Vacuna?> ObtenerPorIdAsync(int id) => Task.FromResult(Vacunas.FirstOrDefault(v => v.Id == id));
}
