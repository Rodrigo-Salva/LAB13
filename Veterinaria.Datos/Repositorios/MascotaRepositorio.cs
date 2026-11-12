using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades.Models;

namespace Veterinaria.Datos.Repositorios;

public interface IMascotaRepositorio
{
    Task<List<Mascota>> ListarAsync(string? especie, bool? activo, decimal? pesoMin, decimal? pesoMax);
    Task<List<Mascota>> ListarPorDuenoAsync(int duenoId);
    Task<Mascota?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteNombreEnDuenoAsync(int duenoId, string nombre, int? excluirId = null);
    Task<int> ContarPorDuenoAsync(int duenoId);
    Task AgregarAsync(Mascota mascota);
    Task ActualizarAsync(Mascota mascota);
    Task EliminarAsync(Mascota mascota);
}

public class MascotaRepositorio : IMascotaRepositorio
{
    private readonly VeterinariaContext _context;
    public MascotaRepositorio(VeterinariaContext context) => _context = context;

    public async Task<List<Mascota>> ListarAsync(string? especie, bool? activo, decimal? pesoMin, decimal? pesoMax)
    {
        var query = _context.Mascotas.Include(m => m.Dueno).AsQueryable();
        if (!string.IsNullOrWhiteSpace(especie)) query = query.Where(m => m.Especie == especie);
        if (activo.HasValue) query = query.Where(m => m.Activo == activo.Value);
        if (pesoMin.HasValue) query = query.Where(m => m.Peso >= pesoMin.Value);
        if (pesoMax.HasValue) query = query.Where(m => m.Peso <= pesoMax.Value);
        return await query.OrderBy(m => m.Nombre).ToListAsync();
    }

    public async Task<List<Mascota>> ListarPorDuenoAsync(int duenoId) =>
        await _context.Mascotas.Include(m => m.Dueno)
            .Where(m => m.DuenoId == duenoId).OrderBy(m => m.Nombre).ToListAsync();

    public Task<Mascota?> ObtenerPorIdAsync(int id) =>
        _context.Mascotas.Include(m => m.Dueno).FirstOrDefaultAsync(m => m.Id == id);

    public Task<bool> ExisteNombreEnDuenoAsync(int duenoId, string nombre, int? excluirId = null) =>
        _context.Mascotas.AnyAsync(m => m.DuenoId == duenoId && m.Nombre == nombre && m.Id != excluirId);

    public Task<int> ContarPorDuenoAsync(int duenoId) =>
        _context.Mascotas.CountAsync(m => m.DuenoId == duenoId);

    public async Task AgregarAsync(Mascota mascota)
    {
        _context.Mascotas.Add(mascota);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Mascota mascota)
    {
        _context.Mascotas.Update(mascota);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Mascota mascota)
    {
        _context.Mascotas.Remove(mascota);
        await _context.SaveChangesAsync();
    }
}
