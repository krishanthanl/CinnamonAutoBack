using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cinnamon.Infrastructure.Repositories;

public class StockMovementRepository : IStockMovementRepository
{
    private readonly AppDbContext _context;

    public StockMovementRepository(AppDbContext context) => _context = context;

    public Task AddAsync(StockMovement movement)
    {
        _context.StockMovements.Add(movement);
        return Task.CompletedTask;
    }

    public Task<List<StockMovement>> GetByProductAsync(int productId, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.StockMovements.AsNoTracking().Where(m => m.ProductId == productId);
        if (from.HasValue) query = query.Where(m => m.MovementDate >= from.Value);
        if (to.HasValue) query = query.Where(m => m.MovementDate <= to.Value);
        return query.OrderBy(m => m.MovementDate).ToListAsync();
    }

    public async Task<int> GetStockAtDateAsync(int productId, DateTime asOfDate)
    {
        var endOfDay = asOfDate.Date.AddDays(1).AddTicks(-1);
        return await _context.StockMovements.AsNoTracking()
            .Where(m => m.ProductId == productId && m.MovementDate <= endOfDay)
            .SumAsync(m => (int?)m.QuantityChange) ?? 0;
    }

    public async Task<Dictionary<int, int>> GetStockAtDateForAllProductsAsync(DateTime asOfDate)
    {
        var endOfDay = asOfDate.Date.AddDays(1).AddTicks(-1);
        return await _context.StockMovements.AsNoTracking()
            .Where(m => m.MovementDate <= endOfDay)
            .GroupBy(m => m.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(m => m.QuantityChange) })
            .ToDictionaryAsync(x => x.ProductId, x => x.Quantity);
    }
}
