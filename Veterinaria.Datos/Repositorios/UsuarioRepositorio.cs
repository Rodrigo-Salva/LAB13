using Microsoft.EntityFrameworkCore;
using Veterinaria.Entidades.Models;

namespace Veterinaria.Datos.Repositorios;

public interface IUsuarioRepositorio
{
    Task<Usuario?> BuscarPorCorreoAsync(string correo);
    Task AgregarAsync(Usuario usuario);
    Task GuardarAsync();
}

public class UsuarioRepositorio : IUsuarioRepositorio
{
    private readonly VeterinariaContext _context;
    public UsuarioRepositorio(VeterinariaContext context) => _context = context;

    public Task<Usuario?> BuscarPorCorreoAsync(string correo) =>
        _context.Usuarios.FirstOrDefaultAsync(u => u.Correo == correo);

    public async Task AgregarAsync(Usuario usuario) => await _context.Usuarios.AddAsync(usuario);

    public Task GuardarAsync() => _context.SaveChangesAsync();
}
