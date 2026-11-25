using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Veterinaria.Datos.Repositorios;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Entidades.Models;
using Veterinaria.Negocio.Excepciones;

namespace Veterinaria.Negocio.Servicios;

/// <summary>Roles propios del negocio.</summary>
public static class Roles
{
    public const string Recepcionista = "Recepcionista";
    public const string Veterinario = "Veterinario";
}

public interface IAuthServicio
{
    Task<UsuarioDto> RegistrarAsync(RegistroDto dto);
    Task<TokenDto?> LoginAsync(LoginDto dto);
}

public class AuthServicio : IAuthServicio
{
    private readonly IUsuarioRepositorio _repo;
    private readonly IConfiguration _config;

    public AuthServicio(IUsuarioRepositorio repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    public async Task<UsuarioDto> RegistrarAsync(RegistroDto dto)
    {
        var correo = dto.Correo.Trim().ToLowerInvariant();

        // Regla de negocio: el correo no puede repetirse
        if (await _repo.BuscarPorCorreoAsync(correo) is not null)
            throw new ReglaNegocioException($"Ya existe un usuario con el correo {correo}.");

        var usuario = new Usuario
        {
            Correo = correo,
            ClaveHash = BCrypt.Net.BCrypt.HashPassword(dto.Clave),
            Rol = Roles.Recepcionista,          // el servidor asigna el rol (el menos privilegiado)
            FechaRegistro = DateTime.Now
        };
        await _repo.AgregarAsync(usuario);
        await _repo.GuardarAsync();

        return new UsuarioDto { Id = usuario.Id, Correo = usuario.Correo, Rol = usuario.Rol };
    }

    // null => credenciales inválidas (no se distingue entre correo inexistente y clave errada)
    public async Task<TokenDto?> LoginAsync(LoginDto dto)
    {
        var usuario = await _repo.BuscarPorCorreoAsync(dto.Correo.Trim().ToLowerInvariant());
        if (usuario is null || !BCrypt.Net.BCrypt.Verify(dto.Clave, usuario.ClaveHash))
            return null;

        return GenerarToken(usuario);
    }

    private TokenDto GenerarToken(Usuario usuario)
    {
        var key = _config["Jwt:Key"];
        if (string.IsNullOrEmpty(key) || key.Length < 32)
            throw new InvalidOperationException("Jwt:Key debe tener al menos 32 caracteres.");

        var minutos = int.Parse(_config["Jwt:Minutos"] ?? "60");
        var expira = DateTime.UtcNow.AddMinutes(minutos);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, usuario.Correo),
            new Claim("role", usuario.Rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var credenciales = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Emisor"],
            audience: _config["Jwt:Audiencia"],
            claims: claims,
            expires: expira,
            signingCredentials: credenciales);

        return new TokenDto { Token = new JwtSecurityTokenHandler().WriteToken(token), Expira = expira };
    }
}
