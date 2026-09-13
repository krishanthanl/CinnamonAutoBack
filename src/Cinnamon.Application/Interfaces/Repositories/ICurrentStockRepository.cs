using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Repositories;

public interface ICurrentStockRepository
{
    Task<List<CurrentStock>> GetAllAsync();
    Task<CurrentStock?> GetByProductIdAsync(int productId);
    Task AddAsync(CurrentStock stock);
    void Update(CurrentStock stock);
}
