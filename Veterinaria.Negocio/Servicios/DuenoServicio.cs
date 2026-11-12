using AutoMapper;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Entidades.Models;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

public interface IDuenoServicio
{
    Task<List<DuenoDto>> ListarAsync(string? nombre);
    Task<DuenoDto?> ObtenerPorIdAsync(int id);
    Task<DuenoDto> CrearAsync(DuenoCrearDto dto);
    Task<bool> ActualizarAsync(int id, DuenoActualizarDto dto);
    Task<bool> EliminarAsync(int id);
}

public class DuenoServicio : IDuenoServicio
{
    private readonly IDuenoRepositorio _repo;
    private readonly IMapper _mapper;

    public DuenoServicio(IDuenoRepositorio repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<DuenoDto>> ListarAsync(string? nombre) =>
        _mapper.Map<List<DuenoDto>>(await _repo.ListarAsync(nombre));

    public async Task<DuenoDto?> ObtenerPorIdAsync(int id)
    {
        var dueno = await _repo.ObtenerPorIdAsync(id);
        return dueno is null ? null : _mapper.Map<DuenoDto>(dueno);
    }

    public async Task<DuenoDto> CrearAsync(DuenoCrearDto dto)
    {
        // Regla de negocio 1: el DNI no puede repetirse
        if (await _repo.ExisteDniAsync(dto.Dni))
            throw new ReglaNegocioException($"Ya existe un dueño con el DNI {dto.Dni}.");

        var dueno = _mapper.Map<Dueno>(dto);
        dueno.FechaRegistro = DateTime.Now;
        await _repo.AgregarAsync(dueno);
        return _mapper.Map<DuenoDto>(dueno);
    }

    public async Task<bool> ActualizarAsync(int id, DuenoActualizarDto dto)
    {
        var dueno = await _repo.ObtenerPorIdAsync(id);
        if (dueno is null) return false;

        _mapper.Map(dto, dueno);   // actualiza la entidad existente
        await _repo.ActualizarAsync(dueno);
        return true;
    }

    public async Task<bool> EliminarAsync(int id)
    {
        var dueno = await _repo.ObtenerPorIdAsync(id);
        if (dueno is null) return false;

        // Regla de negocio 2: no se elimina un dueño que aún tiene mascotas
        if (dueno.Mascotas.Any())
            throw new ReglaNegocioException("No se puede eliminar un dueño que tiene mascotas registradas.");

        await _repo.EliminarAsync(dueno);
        return true;
    }
}
