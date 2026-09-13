using Cinnamon.Domain.Entities;
namespace Cinnamon.Application.Interfaces.Repositories;
public interface IBrandRepository
{
    Task<List<Brand>> SearchAsync(string? search);
    Task<Brand?> GetByIdAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task AddAsync(Brand brand);
    void Update(Brand brand);
    void Delete(Brand brand);
}
