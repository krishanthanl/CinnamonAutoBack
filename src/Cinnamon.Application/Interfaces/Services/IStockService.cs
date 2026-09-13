using Cinnamon.Application.DTOs;

namespace Cinnamon.Application.Interfaces.Services;

public interface IStockService
{
    Task<StockMovementDto> RecordMovementAsync(CreateStockMovementDto dto);
    Task<List<ProductStockAtDateDto>> GetCurrentStockAsync();
    Task<ProductStockAtDateDto> GetStockAtDateAsync(int productId, DateTime asOfDate);
    Task<List<ProductStockAtDateDto>> GetAllStockAtDateAsync(DateTime asOfDate);
    Task<List<StockMovementDto>> GetMovementHistoryAsync(int productId, DateTime? from = null, DateTime? to = null);
}
