using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cinnamon.Infrastructure.Repositories;

public class CurrentStockRepository : ICurrentStockRepository
{
    private readonly AppDbContext _context;

    public CurrentStockRepository(AppDbContext context) => _context = context;

    public Task<List<CurrentStock>> GetAllAsync() =>
        _context.CurrentStocks.AsNoTracking().ToListAsync();

    public Task<CurrentStock?> GetByProductIdAsync(int productId) =>
        _context.CurrentStocks.FirstOrDefaultAsync(s => s.ProductId == productId);

    public Task AddAsync(CurrentStock stock)
    {
        _context.CurrentStocks.Add(stock);
        return Task.CompletedTask;
    }

    public void Update(CurrentStock stock) => _context.CurrentStocks.Update(stock);
}
