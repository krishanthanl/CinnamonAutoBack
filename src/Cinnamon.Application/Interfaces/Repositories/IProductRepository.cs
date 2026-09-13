using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Repositories;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Task<Product?> GetByIdAsync(int id);
    Task<List<Product>> GetByCategoryAsync(int categoryId);
    Task<List<Product>> SearchAsync(int? categoryId, string? search);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Product product);
    void Update(Product product);
    void Delete(Product product);
}
