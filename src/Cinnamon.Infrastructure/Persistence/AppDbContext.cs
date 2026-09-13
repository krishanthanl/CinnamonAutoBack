using Cinnamon.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cinnamon.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<CurrentStock> CurrentStocks => Set<CurrentStock>();
    public DbSet<StockMovement> StockMovements => Set<StockMovement>();
    public DbSet<Brand> Brands => Set<Brand>();
    public DbSet<VehicleModel> VehicleModels => Set<VehicleModel>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
