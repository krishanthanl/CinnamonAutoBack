using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Cinnamon.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public Task<List<Product>> GetAllAsync() =>
        _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Brand).Include(p => p.VehicleModel).ToListAsync();

    public Task<Product?> GetByIdAsync(int id) =>
        _context.Products.Include(p => p.Category).Include(p => p.Brand).Include(p => p.VehicleModel).FirstOrDefaultAsync(p => p.Id == id);

    public Task<List<Product>> GetByCategoryAsync(int categoryId) =>
        _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Brand).Include(p => p.VehicleModel)
            .Where(p => p.CategoryId == categoryId).ToListAsync();

    public Task<List<Product>> SearchAsync(int? categoryId, string? search)
    {
        var query = _context.Products.AsNoTracking().Include(p => p.Category).Include(p => p.Brand).Include(p => p.VehicleModel).AsQueryable();

        if (categoryId.HasValue)
        {
            query = query.Where(product => product.CategoryId == categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(product =>
                product.Name.Contains(term) ||
                product.Description.Contains(term) ||
                product.Category!.Name.Contains(term) ||
                (product.Brand != null && product.Brand.Name.Contains(term)) ||
                (product.VehicleModel != null && (product.VehicleModel.Name.Contains(term) || product.VehicleModel.Year.ToString().Contains(term))));
        }

        return query.OrderBy(product => product.Name).ToListAsync();
    }

    public Task<bool> ExistsAsync(int id) =>
        _context.Products.AnyAsync(p => p.Id == id);

    public Task AddAsync(Product product)
    {
        _context.Products.Add(product);
        return Task.CompletedTask;
    }

    public void Update(Product product) => _context.Products.Update(product);

    public void Delete(Product product)
    {
        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        _context.Products.Update(product);
    }
}
