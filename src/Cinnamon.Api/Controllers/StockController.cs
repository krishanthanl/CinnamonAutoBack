using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Cinnamon.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockController : ControllerBase
{
    private readonly IStockService _stockService;

    public StockController(IStockService stockService) => _stockService = stockService;

    [HttpGet("current")]
    public async Task<ActionResult<List<ProductStockAtDateDto>>> GetCurrent() =>
        Ok(await _stockService.GetCurrentStockAsync());

    [HttpGet("at-date")]
    public async Task<ActionResult<List<ProductStockAtDateDto>>> GetAllAtDate([FromQuery] DateTime date) =>
        Ok(await _stockService.GetAllStockAtDateAsync(date));

    [HttpGet("products/{productId:int}/at-date")]
    public async Task<ActionResult<ProductStockAtDateDto>> GetProductAtDate(int productId, [FromQuery] DateTime date) =>
        Ok(await _stockService.GetStockAtDateAsync(productId, date));

    [HttpGet("products/{productId:int}/history")]
    public async Task<ActionResult<List<StockMovementDto>>> GetHistory(int productId, [FromQuery] DateTime? from, [FromQuery] DateTime? to) =>
        Ok(await _stockService.GetMovementHistoryAsync(productId, from, to));

    [HttpPost("movements")]
    public async Task<ActionResult<StockMovementDto>> RecordMovement([FromBody] CreateStockMovementDto dto) =>
        Ok(await _stockService.RecordMovementAsync(dto));
}
