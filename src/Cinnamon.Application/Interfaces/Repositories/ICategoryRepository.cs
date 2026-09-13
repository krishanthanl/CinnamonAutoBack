using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task<Category?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Category category);
    void Update(Category category);
    void Delete(Category category);
}
