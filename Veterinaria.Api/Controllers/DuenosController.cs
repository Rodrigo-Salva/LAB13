using Microsoft.AspNetCore.Mvc;
using Veterinaria.Entidades.Dtos;
using Veterinaria.Negocio.Excepciones;
using Veterinaria.Negocio.Servicios;

namespace Veterinaria.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DuenosController : ControllerBase
{
    private readonly IDuenoServicio _duenos;
    private readonly IMascotaServicio _mascotas;

    public DuenosController(IDuenoServicio duenos, IMascotaServicio mascotas)
    {
        _duenos = duenos;
        _mascotas = mascotas;
    }

    /// <summary>Lista dueños; filtro opcional ?nombre=</summary>
    [HttpGet]
    public async Task<ActionResult<List<DuenoDto>>> Get([FromQuery] string? nombre) =>
        Ok(await _duenos.ListarAsync(nombre));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DuenoDto>> GetPorId(int id)
    {
        var dueno = await _duenos.ObtenerPorIdAsync(id);
        return dueno is null ? NotFound() : Ok(dueno);
    }

    /// <summary>Ruta anidada: mascotas de un dueño</summary>
    [HttpGet("{id:int}/mascotas")]
    public async Task<ActionResult<List<MascotaDto>>> GetMascotas(int id)
    {
        var lista = await _mascotas.ListarPorDuenoAsync(id);
        return lista is null ? NotFound() : Ok(lista);
    }

    [HttpPost]
    public async Task<ActionResult<DuenoDto>> Post(DuenoCrearDto dto)
    {
        try
        {
            var creado = await _duenos.CrearAsync(dto);
            return CreatedAtAction(nameof(GetPorId), new { id = creado.Id }, creado);
        }
        catch (ReglaNegocioException ex) { return Conflict(new { mensaje = ex.Message }); }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Put(int id, DuenoActualizarDto dto) =>
        await _duenos.ActualizarAsync(id, dto) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        try { return await _duenos.EliminarAsync(id) ? NoContent() : NotFound(); }
        catch (ReglaNegocioException ex) { return Conflict(new { mensaje = ex.Message }); }
    }
}
