using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;
namespace Cinnamon.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandService _service;
    public BrandsController(IBrandService service) => _service = service;
    [HttpGet] public async Task<ActionResult<List<BrandDto>>> Search([FromQuery] string? search) => Ok(await _service.SearchAsync(search));
    [HttpGet("{id:int}")] public async Task<ActionResult<BrandDto>> Get(int id) => Ok(await _service.GetByIdAsync(id));
    [HttpPost] public async Task<ActionResult<BrandDto>> Create(SaveBrandDto dto) { var item = await _service.CreateAsync(dto); return CreatedAtAction(nameof(Get), new { id = item.Id }, item); }
    [HttpPut("{id:int}")] public async Task<ActionResult<BrandDto>> Update(int id, SaveBrandDto dto) => Ok(await _service.UpdateAsync(id, dto));
    [HttpDelete("{id:int}")] public async Task<IActionResult> Delete(int id) { await _service.DeleteAsync(id); return NoContent(); }
}
