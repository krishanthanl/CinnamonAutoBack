using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Cinnamon.Infrastructure.Repositories;
public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;
    public BrandRepository(AppDbContext context) => _context = context;
    public Task<List<Brand>> SearchAsync(string? search)
    {
        var query = _context.Brands.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search)) query = query.Where(brand => brand.Name.Contains(search.Trim()));
        return query.OrderBy(brand => brand.Name).ToListAsync();
    }
    public Task<Brand?> GetByIdAsync(int id) => _context.Brands.FirstOrDefaultAsync(brand => brand.Id == id);
    public Task<bool> ExistsAsync(int id) => _context.Brands.AnyAsync(brand => brand.Id == id);
    public Task AddAsync(Brand brand) { _context.Brands.Add(brand); return Task.CompletedTask; }
    public void Update(Brand brand) => _context.Brands.Update(brand);
    public void Delete(Brand brand) { brand.IsDeleted = true; brand.DeletedAt = DateTime.UtcNow; _context.Brands.Update(brand); }
}
