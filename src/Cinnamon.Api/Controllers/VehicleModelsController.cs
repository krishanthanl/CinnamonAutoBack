using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
namespace Cinnamon.Api.Controllers;
[ApiController]
[Route("api/vehicle-models")]
public class VehicleModelsController : ControllerBase
{
    private readonly IVehicleModelService _service;
    public VehicleModelsController(IVehicleModelService service) => _service = service;
    [HttpGet] public async Task<ActionResult<List<VehicleModelDto>>> Search([FromQuery] int? brandId, [FromQuery] string? search) => Ok(await _service.SearchAsync(brandId, search));
    [HttpGet("{id:int}")] public async Task<ActionResult<VehicleModelDto>> Get(int id) => Ok(await _service.GetByIdAsync(id));
    [HttpPost] public async Task<ActionResult<VehicleModelDto>> Create(SaveVehicleModelDto dto) { var item = await _service.CreateAsync(dto); return CreatedAtAction(nameof(Get), new { id = item.Id }, item); }
    [HttpPut("{id:int}")] public async Task<ActionResult<VehicleModelDto>> Update(int id, SaveVehicleModelDto dto) => Ok(await _service.UpdateAsync(id, dto));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return NoContent(); }
}
