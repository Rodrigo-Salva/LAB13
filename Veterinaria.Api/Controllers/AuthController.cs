using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthServicio _auth;
    public AuthController(IAuthServicio auth) => _auth = auth;

    [AllowAnonymous]
    [HttpPost("registro")]
    public async Task<ActionResult<UsuarioDto>> Registro(RegistroDto dto)
    {
        try
        {
            var usuario = await _auth.RegistrarAsync(dto);
            return StatusCode(StatusCodes.Status201Created, usuario);
        }
        catch (ReglaNegocioException ex) { return Conflict(new { mensaje = ex.Message }); }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenDto>> Login(LoginDto dto)
    {
        var token = await _auth.LoginAsync(dto);
        return token is null ? Unauthorized(new { mensaje = "Credenciales inválidas." }) : Ok(token);
    }

    /// <summary>Datos tomados de los claims del token, no de la base de datos.</summary>
    [Authorize]
    [HttpGet("perfil")]
    public ActionResult<PerfilDto> Perfil() => Ok(new PerfilDto
    {
        Correo = User.Claims.First(c => c.Type == "email").Value,
        Rol = User.Claims.First(c => c.Type == "role").Value
    });
}
