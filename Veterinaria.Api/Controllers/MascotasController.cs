using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class MascotasController : ControllerBase
{
    private readonly IMascotaServicio _servicio;
    public MascotasController(IMascotaServicio servicio) => _servicio = servicio;

    /// <summary>Lista mascotas con filtros: ?especie=&amp;activo=&amp;pesoMin=&amp;pesoMax=</summary>
    [AllowAnonymous]
    [HttpGet]
    public async Task<ActionResult<List<MascotaDto>>> Get(
        [FromQuery] string? especie, [FromQuery] bool? activo,
        [FromQuery] decimal? pesoMin, [FromQuery] decimal? pesoMax) =>
        Ok(await _servicio.ListarAsync(especie, activo, pesoMin, pesoMax));

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<ActionResult<MascotaDto>> GetPorId(int id)
    {
        var mascota = await _servicio.ObtenerPorIdAsync(id);
        return mascota is null ? NotFound() : Ok(mascota);
    }

    [HttpPost]
    public async Task<ActionResult<MascotaDto>> Post(MascotaCrearDto dto)
    {
        try
        {
            var creada = await _servicio.CrearAsync(dto);
            return creada is null
                ? NotFound(new { mensaje = $"No existe el dueño {dto.DuenoId}." })
                : CreatedAtAction(nameof(GetPorId), new { id = creada.Id }, creada);
        }
        catch (ReglaNegocioException ex) { return Conflict(new { mensaje = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, MascotaActualizarDto dto)
    {
        try { return await _servicio.ActualizarAsync(id, dto) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException ex) { return Conflict(new { mensaje = ex.Message }); }
    }

    [Authorize(Roles = Roles.Veterinario)]
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id) =>
        await _servicio.EliminarAsync(id) ? NoContent() : NotFound();
}
