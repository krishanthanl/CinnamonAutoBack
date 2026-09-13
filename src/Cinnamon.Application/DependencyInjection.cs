using Cinnamon.Application.Interfaces.Services;
using Cinnamon.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Cinnamon.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IStockService, StockService>();
        services.AddScoped<IBrandService, BrandService>();
        services.AddScoped<IVehicleModelService, VehicleModelService>();
        return services;
    }
}
