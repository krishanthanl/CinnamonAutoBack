using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Repositories;

public interface IStockMovementRepository
{
    Task AddAsync(StockMovement movement);
    Task<List<StockMovement>> GetByProductAsync(int productId, DateTime? from = null, DateTime? to = null);

    /// <summary>Sum of QuantityChange for a product across all movements with MovementDate &lt;= asOfDate (inclusive, end-of-day).</summary>
    Task<int> GetStockAtDateAsync(int productId, DateTime asOfDate);

    /// <summary>Same as GetStockAtDateAsync but for every product at once, keyed by ProductId.</summary>
    Task<Dictionary<int, int>> GetStockAtDateForAllProductsAsync(DateTime asOfDate);
}
