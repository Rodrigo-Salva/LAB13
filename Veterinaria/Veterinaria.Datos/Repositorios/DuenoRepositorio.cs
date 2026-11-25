using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades.Models;

namespace Veterinaria.Datos.Repositorios;

public interface IDuenoRepositorio
{
    Task<List<Dueno>> ListarAsync(string? nombre);
    Task<Dueno?> ObtenerPorIdAsync(int id);
    Task<bool> ExisteDniAsync(string dni);
    Task AgregarAsync(Dueno dueno);
    Task ActualizarAsync(Dueno dueno);
    Task EliminarAsync(Dueno dueno);
}

public class DuenoRepositorio : IDuenoRepositorio
{
    private readonly VeterinariaContext _context;
    public DuenoRepositorio(VeterinariaContext context) => _context = context;

    public async Task<List<Dueno>> ListarAsync(string? nombre)
    {
        var query = _context.Duenos.Include(d => d.Mascotas).AsQueryable();
        if (!string.IsNullOrWhiteSpace(nombre))
            query = query.Where(d => d.Nombre.Contains(nombre));
        return await query.OrderBy(d => d.Nombre).ToListAsync();
    }

    public Task<Dueno?> ObtenerPorIdAsync(int id) =>
        _context.Duenos.Include(d => d.Mascotas).FirstOrDefaultAsync(d => d.Id == id);

    public Task<bool> ExisteDniAsync(string dni) => _context.Duenos.AnyAsync(d => d.Dni == dni);

    public async Task AgregarAsync(Dueno dueno)
    {
        _context.Duenos.Add(dueno);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarAsync(Dueno dueno)
    {
        _context.Duenos.Update(dueno);
        await _context.SaveChangesAsync();
    }

    public async Task EliminarAsync(Dueno dueno)
    {
        _context.Duenos.Remove(dueno);
        await _context.SaveChangesAsync();
    }
}
