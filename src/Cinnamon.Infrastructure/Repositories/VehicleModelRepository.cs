using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Domain.Entities;
using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Cinnamon.Infrastructure.Repositories;
public class VehicleModelRepository : IVehicleModelRepository
{
    private readonly AppDbContext _context;
    public VehicleModelRepository(AppDbContext context) => _context = context;
    public Task<List<VehicleModel>> SearchAsync(int? brandId, string? search)
    {
        var query = _context.VehicleModels.AsNoTracking().Include(model => model.Brand).AsQueryable();
        if (brandId.HasValue) query = query.Where(model => model.BrandId == brandId.Value);
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(model => model.Name.Contains(term) || model.Brand!.Name.Contains(term) || model.Year.ToString().Contains(term));
        }
        return query.OrderBy(model => model.Brand!.Name).ThenBy(model => model.Name).ThenByDescending(model => model.Year).ToListAsync();
    }
    public Task<VehicleModel?> GetByIdAsync(int id) => _context.VehicleModels.Include(model => model.Brand).FirstOrDefaultAsync(model => model.Id == id);
    public Task AddAsync(VehicleModel model) { _context.VehicleModels.Add(model); return Task.CompletedTask; }
    public void Update(VehicleModel model) => _context.VehicleModels.Update(model);
    public void Delete(VehicleModel model) { model.IsDeleted = true; model.DeletedAt = DateTime.UtcNow; _context.VehicleModels.Update(model); }
}
