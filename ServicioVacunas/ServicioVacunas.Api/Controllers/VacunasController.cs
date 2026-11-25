using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServicioVacunas.Entidades.Dtos;
using ServicioVacunas.Negocio.Servicios;

namespace ServicioVacunas.Api.Controllers;

[ApiController]
[Authorize]   // todos los endpoints exigen token
[Route("api/[controller]")]
public class VacunasController : ControllerBase
{
    private readonly IVacunaServicio _servicio;
    public VacunasController(IVacunaServicio servicio) => _servicio = servicio;

    [HttpGet]
    public async Task<ActionResult<List<VacunaDto>>> Get() => Ok(await _servicio.ListarAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<VacunaDto>> GetPorId(int id)
    {
        var vacuna = await _servicio.ObtenerPorIdAsync(id);
        return vacuna is null ? NotFound() : Ok(vacuna);
    }
}
