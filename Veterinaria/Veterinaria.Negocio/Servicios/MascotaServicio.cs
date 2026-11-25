using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Entidades.Models;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public interface IMascotaServicio
{
    Task<List<MascotaDto>> ListarAsync(string? especie, bool? activo, decimal? pesoMin, decimal? pesoMax);
    Task<List<MascotaDto>?> ListarPorDuenoAsync(int duenoId);
    Task<MascotaDto?> ObtenerPorIdAsync(int id);
    Task<MascotaDto?> CrearAsync(MascotaCrearDto dto);
    Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}

public class MascotaServicio : IMascotaServicio
{
    private const int MaxMascotasPorDueno = 5;

    private readonly IMascotaRepositorio _repo;
    private readonly IDuenoRepositorio _duenoRepo;
    private readonly IMapper _mapper;

    public MascotaServicio(IMascotaRepositorio repo, IDuenoRepositorio duenoRepo, IMapper mapper)
    {
        _repo = repo;
        _duenoRepo = duenoRepo;
        _mapper = mapper;
    }

    public async Task<List<MascotaDto>> ListarAsync(string? especie, bool? activo, decimal? pesoMin, decimal? pesoMax) =>
        _mapper.Map<List<MascotaDto>>(await _repo.ListarAsync(especie, activo, pesoMin, pesoMax));

    // null => el dueño no existe (el controlador responde 404)
    public async Task<List<MascotaDto>?> ListarPorDuenoAsync(int duenoId)
    {
        if (await _duenoRepo.ObtenerPorIdAsync(duenoId) is null) return null;
        return _mapper.Map<List<MascotaDto>>(await _repo.ListarPorDuenoAsync(duenoId));
    }

    public async Task<MascotaDto?> ObtenerPorIdAsync(int id)
    {
        var mascota = await _repo.ObtenerPorIdAsync(id);
        return mascota is null ? null : _mapper.Map<MascotaDto>(mascota);
    }

    // null => el dueño no existe
    public async Task<MascotaDto?> CrearAsync(MascotaCrearDto dto)
    {
        if (await _duenoRepo.ObtenerPorIdAsync(dto.DuenoId) is null) return null;

        // Regla de negocio 3: máximo 5 mascotas por dueño
        if (await _repo.ContarPorDuenoAsync(dto.DuenoId) >= MaxMascotasPorDueno)
            throw new ReglaNegocioException($"Un dueño no puede tener más de {MaxMascotasPorDueno} mascotas.");

        // Regla de negocio 4: un dueño no puede repetir el nombre de sus mascotas
        if (await _repo.ExisteNombreEnDuenoAsync(dto.DuenoId, dto.Nombre))
            throw new ReglaNegocioException($"El dueño ya tiene una mascota llamada '{dto.Nombre}'.");

        var mascota = _mapper.Map<Mascota>(dto);
        mascota.Activo = true;
        await _repo.AgregarAsync(mascota);

        var creada = await _repo.ObtenerPorIdAsync(mascota.Id);   // carga el Dueño para DuenoNombre
        return _mapper.Map<MascotaDto>(creada);
    }

    public async Task<bool> ActualizarAsync(int id, MascotaActualizarDto dto)
    {
        var mascota = await _repo.ObtenerPorIdAsync(id);
        if (mascota is null) return false;

        if (await _repo.ExisteNombreEnDuenoAsync(mascota.DuenoId, dto.Nombre, id))
            throw new ReglaNegocioException($"El dueño ya tiene una mascota llamada '{dto.Nombre}'.");

        _mapper.Map(dto, mascota);
        await _repo.ActualizarAsync(mascota);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var mascota = await _repo.ObtenerPorIdAsync(id);
        if (mascota is null) return false;
        await _repo.EliminarAsync(mascota);
        return true;
    }
}
