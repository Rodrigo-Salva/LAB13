using ServicioVacunas.Datos.Repositorios;
using ServicioVacunas.Entidades.Dtos;
using ServicioVacunas.Entidades.Models;

namespace ServicioVacunas.Negocio.Servicios;

public interface IVacunaServicio
{
    Task<List<VacunaDto>> ListarAsync();
    Task<VacunaDto?> ObtenerPorIdAsync(int id);
}

public class VacunaServicio : IVacunaServicio
{
    private readonly IVacunaRepositorio _repo;
    public VacunaServicio(IVacunaRepositorio repo) => _repo = repo;

    public async Task<List<VacunaDto>> ListarAsync() =>
        (await _repo.ListarAsync()).Select(ADto).ToList();

    public async Task<VacunaDto?> ObtenerPorIdAsync(int id)
    {
        var vacuna = await _repo.ObtenerPorIdAsync(id);
        return vacuna is null ? null : ADto(vacuna);
    }

    // La API solo expone DTOs, nunca la entidad
    private static VacunaDto ADto(Vacuna v) => new(v.Id, v.Nombre, v.Especie, v.Frecuencia, v.Precio);
}
