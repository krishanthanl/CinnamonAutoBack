using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Infrastructure.Persistence;
using Cinnamon.Infrastructure.Repositories;
using Cinnamon.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Cinnamon.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration, string imagesFolder)
    {
        var connectionString = configuration.GetConnectionString("SqlServer")
            ?? throw new InvalidOperationException("Missing required configuration: ConnectionStrings:SqlServer");

        services.AddDbContext<AppDbContext>(options => options.UseSqlServer(connectionString));

        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICurrentStockRepository, CurrentStockRepository>();
        services.AddScoped<IStockMovementRepository, StockMovementRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();
        services.AddScoped<IVehicleModelRepository, VehicleModelRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddSingleton<IImageStorageService>(new FileSystemImageStorageService(imagesFolder));

        return services;
    }
}
