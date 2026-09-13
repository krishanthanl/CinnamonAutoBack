using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cinnamon.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context) => _context = context;

    public Task<List<Category>> GetAllAsync() =>
        _context.Categories.AsNoTracking().ToListAsync();

    public Task<Category?> GetByIdAsync(int id) =>
        _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

    public Task<bool> ExistsAsync(int id) =>
        _context.Categories.AnyAsync(c => c.Id == id);

    public Task AddAsync(Category category)
    {
        _context.Categories.Add(category);
        return Task.CompletedTask;
    }

    public void Update(Category category) => _context.Categories.Update(category);

    public void Delete(Category category)
    {
        category.IsDeleted = true;
        category.DeletedAt = DateTime.UtcNow;
        _context.Categories.Update(category);
    }
}
