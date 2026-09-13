using Cinnamon.Application.Common;
using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Application.Interfaces.Services;
using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly ICategoryRepository _categories;
    private readonly IBrandRepository _brands;
    private readonly IVehicleModelRepository _vehicleModels;
    private readonly ICurrentStockRepository _currentStock;
    private readonly IStockMovementRepository _stockMovements;
    private readonly IImageStorageService _imageStorage;
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(
        IProductRepository products,
        ICategoryRepository categories,
        IBrandRepository brands,
        IVehicleModelRepository vehicleModels,
        ICurrentStockRepository currentStock,
        IStockMovementRepository stockMovements,
        IImageStorageService imageStorage,
        IUnitOfWork unitOfWork)
    {
        _products = products;
        _categories = categories;
        _brands = brands;
        _vehicleModels = vehicleModels;
        _currentStock = currentStock;
        _stockMovements = stockMovements;
        _imageStorage = imageStorage;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ProductDto>> GetAllAsync()
    {
        var products = await _products.GetAllAsync();
        return await ToDtosAsync(products);
    }

    public async Task<ProductDto> GetByIdAsync(int id)
    {
        var product = await _products.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Product), id);
        var dtos = await ToDtosAsync(new List<Product> { product });
        return dtos[0];
    }

    public async Task<List<ProductDto>> GetByCategoryAsync(int categoryId)
    {
        var products = await _products.GetByCategoryAsync(categoryId);
        return await ToDtosAsync(products);
    }

    public async Task<List<ProductDto>> SearchAsync(int? categoryId, string? search)
    {
        var products = await _products.SearchAsync(categoryId, search);
        return await ToDtosAsync(products);
    }

    public async Task<ProductDto> CreateAsync(CreateProductDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException("Product name is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.Description))
        {
            throw new ValidationException("Product description is required.");
        }

        if (string.IsNullOrWhiteSpace(dto.ImageBase64))
        {
            throw new ValidationException("Product image is required.");
        }

        if (!await _categories.ExistsAsync(dto.CategoryId))
        {
            throw new NotFoundException(nameof(Category), dto.CategoryId);
        }

        if (dto.InitialQuantity < 0)
        {
            throw new ValidationException("Initial quantity cannot be negative.");
        }

        ValidatePrices(dto.CostPrice, dto.SellingPrice);
        await ValidateVehicleAsync(dto.BrandId, dto.VehicleModelId);

        var product = new Product
        {
            Name = dto.Name.Trim(),
            CategoryId = dto.CategoryId,
            BrandId = dto.BrandId,
            VehicleModelId = dto.VehicleModelId,
            Description = dto.Description.Trim(),
            ReorderLevel = dto.ReorderLevel,
            ReorderAmount = dto.ReorderAmount,
            CostPrice = dto.CostPrice,
            SellingPrice = dto.SellingPrice
        };

        await _products.AddAsync(product);
        await _unitOfWork.SaveChangesAsync(); // assigns product.Id

        var imageUrl = await _imageStorage.SaveAsync(dto.ImageBase64, $"product-{product.Id}", dto.ImageExtension);
        product.ImageSrc = imageUrl;
        _products.Update(product);

        await _currentStock.AddAsync(new CurrentStock
        {
            ProductId = product.Id,
            Quantity = dto.InitialQuantity,
            LastUpdated = DateTime.UtcNow
        });

        if (dto.InitialQuantity > 0)
        {
            await _stockMovements.AddAsync(new StockMovement
            {
                ProductId = product.Id,
                MovementType = StockMovementType.AdminAdjustment,
                QuantityChange = dto.InitialQuantity,
                MovementDate = DateTime.UtcNow,
                Notes = "Initial stock on product creation"
            });
        }

        await _unitOfWork.SaveChangesAsync();

        var dtos = await ToDtosAsync(new List<Product> { product });
        return dtos[0];
    }

    public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
    {
        var product = await _products.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Product), id);

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException("Product name is required.");
        }

        if (!await _categories.ExistsAsync(dto.CategoryId))
        {
            throw new NotFoundException(nameof(Category), dto.CategoryId);
        }

        if (dto.CurrentQuantity < 0)
        {
            throw new ValidationException("Current quantity cannot be negative.");
        }

        ValidatePrices(dto.CostPrice, dto.SellingPrice);
        await ValidateVehicleAsync(dto.BrandId, dto.VehicleModelId);

        product.Name = dto.Name.Trim();
        product.CategoryId = dto.CategoryId;
        product.BrandId = dto.BrandId;
        product.VehicleModelId = dto.VehicleModelId;
        product.Description = dto.Description.Trim();
        product.ReorderLevel = dto.ReorderLevel;
        product.ReorderAmount = dto.ReorderAmount;
        product.CostPrice = dto.CostPrice;
        product.SellingPrice = dto.SellingPrice;

        if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
        {
            product.ImageSrc = await _imageStorage.SaveAsync(
                dto.ImageBase64,
                $"product-{product.Id}",
                dto.ImageExtension);
        }

        var stock = await _currentStock.GetByProductIdAsync(product.Id);
        var previousQuantity = stock?.Quantity ?? 0;
        var quantityChange = dto.CurrentQuantity - previousQuantity;
        if (stock is null)
        {
            await _currentStock.AddAsync(new CurrentStock
            {
                ProductId = product.Id,
                Quantity = dto.CurrentQuantity,
                LastUpdated = DateTime.UtcNow
            });
        }
        else if (quantityChange != 0)
        {
            stock.Quantity = dto.CurrentQuantity;
            stock.LastUpdated = DateTime.UtcNow;
            _currentStock.Update(stock);
        }

        if (quantityChange != 0)
        {
            await _stockMovements.AddAsync(new StockMovement
            {
                ProductId = product.Id,
                MovementType = StockMovementType.AdminAdjustment,
                QuantityChange = quantityChange,
                MovementDate = DateTime.UtcNow,
                Notes = "Stock updated while editing product"
            });
        }

        _products.Update(product);
        await _unitOfWork.SaveChangesAsync();

        var dtos = await ToDtosAsync(new List<Product> { product });
        return dtos[0];
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _products.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Product), id);
        _products.Delete(product);
        await _unitOfWork.SaveChangesAsync();
    }

    private async Task<List<ProductDto>> ToDtosAsync(List<Product> products)
    {
        var result = new List<ProductDto>(products.Count);
        foreach (var product in products)
        {
            var stock = await _currentStock.GetByProductIdAsync(product.Id);
            result.Add(new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                CategoryId = product.CategoryId,
                CategoryName = product.Category?.Name,
                BrandId = product.BrandId,
                BrandName = product.Brand?.Name,
                VehicleModelId = product.VehicleModelId,
                VehicleModelName = product.VehicleModel?.Name,
                VehicleModelYear = product.VehicleModel?.Year,
                ImageSrc = product.ImageSrc,
                Description = product.Description,
                ReorderLevel = product.ReorderLevel,
                ReorderAmount = product.ReorderAmount,
                CurrentQuantity = stock?.Quantity ?? 0,
                CostPrice = product.CostPrice,
                SellingPrice = product.SellingPrice
            });
        }
        return result;
    }

    private static void ValidatePrices(decimal costPrice, decimal sellingPrice)
    {
        if (costPrice < 0 || sellingPrice < 0)
        {
            throw new ValidationException("Cost price and selling price cannot be negative.");
        }
    }

    private async Task ValidateVehicleAsync(int? brandId, int? vehicleModelId)
    {
        if (brandId.HasValue && !await _brands.ExistsAsync(brandId.Value))
        {
            throw new NotFoundException(nameof(Brand), brandId.Value);
        }

        if (!vehicleModelId.HasValue)
        {
            return;
        }

        if (!brandId.HasValue)
        {
            throw new ValidationException("A brand is required when a vehicle model is selected.");
        }

        var model = await _vehicleModels.GetByIdAsync(vehicleModelId.Value)
            ?? throw new NotFoundException(nameof(VehicleModel), vehicleModelId.Value);
        if (model.BrandId != brandId.Value)
        {
            throw new ValidationException("The selected vehicle model does not belong to the selected brand.");
        }
    }
}
