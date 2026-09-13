using Cinnamon.Application.Common;
using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Application.Interfaces.Services;
using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Services;

public class StockService : IStockService
{
    private readonly IProductRepository _products;
    private readonly ICurrentStockRepository _currentStock;
    private readonly IStockMovementRepository _stockMovements;
    private readonly IUnitOfWork _unitOfWork;

    public StockService(
        IProductRepository products,
        ICurrentStockRepository currentStock,
        IStockMovementRepository stockMovements,
        IUnitOfWork unitOfWork)
    {
        _products = products;
        _currentStock = currentStock;
        _stockMovements = stockMovements;
        _unitOfWork = unitOfWork;
    }

    public async Task<StockMovementDto> RecordMovementAsync(CreateStockMovementDto dto)
    {
        var product = await _products.GetByIdAsync(dto.ProductId) ?? throw new NotFoundException(nameof(Product), dto.ProductId);

        if (dto.QuantityChange == 0)
        {
            throw new ValidationException("Quantity change cannot be zero.");
        }

        var stock = await _currentStock.GetByProductIdAsync(product.Id);
        var newQuantity = (stock?.Quantity ?? 0) + dto.QuantityChange;
        if (newQuantity < 0)
        {
            throw new ValidationException($"Insufficient stock for product '{product.Name}'. Available: {stock?.Quantity ?? 0}, requested change: {dto.QuantityChange}.");
        }

        var movementDate = dto.MovementDate ?? DateTime.UtcNow;

        var movement = new StockMovement
        {
            ProductId = product.Id,
            MovementType = dto.MovementType,
            QuantityChange = dto.QuantityChange,
            MovementDate = movementDate,
            Notes = dto.Notes
        };
        await _stockMovements.AddAsync(movement);

        if (stock is null)
        {
            await _currentStock.AddAsync(new CurrentStock { ProductId = product.Id, Quantity = newQuantity, LastUpdated = movementDate });
        }
        else
        {
            stock.Quantity = newQuantity;
            stock.LastUpdated = movementDate;
            _currentStock.Update(stock);
        }

        await _unitOfWork.SaveChangesAsync();

        return new StockMovementDto
        {
            Id = movement.Id,
            ProductId = movement.ProductId,
            MovementType = movement.MovementType,
            QuantityChange = movement.QuantityChange,
            MovementDate = movement.MovementDate,
            Notes = movement.Notes
        };
    }

    public async Task<List<ProductStockAtDateDto>> GetCurrentStockAsync()
    {
        var stocks = await _currentStock.GetAllAsync();
        var products = await _products.GetAllAsync();
        var productNames = products.ToDictionary(p => p.Id, p => p.Name);

        return stocks.Select(s => new ProductStockAtDateDto
        {
            ProductId = s.ProductId,
            ProductName = productNames.GetValueOrDefault(s.ProductId, "Unknown"),
            AsOfDate = s.LastUpdated,
            Quantity = s.Quantity
        }).ToList();
    }

    public async Task<ProductStockAtDateDto> GetStockAtDateAsync(int productId, DateTime asOfDate)
    {
        var product = await _products.GetByIdAsync(productId) ?? throw new NotFoundException(nameof(Product), productId);
        var quantity = await _stockMovements.GetStockAtDateAsync(productId, asOfDate);

        return new ProductStockAtDateDto
        {
            ProductId = product.Id,
            ProductName = product.Name,
            AsOfDate = asOfDate,
            Quantity = quantity
        };
    }

    public async Task<List<ProductStockAtDateDto>> GetAllStockAtDateAsync(DateTime asOfDate)
    {
        var products = await _products.GetAllAsync();
        var quantities = await _stockMovements.GetStockAtDateForAllProductsAsync(asOfDate);

        return products.Select(p => new ProductStockAtDateDto
        {
            ProductId = p.Id,
            ProductName = p.Name,
            AsOfDate = asOfDate,
            Quantity = quantities.GetValueOrDefault(p.Id, 0)
        }).ToList();
    }

    public async Task<List<StockMovementDto>> GetMovementHistoryAsync(int productId, DateTime? from = null, DateTime? to = null)
    {
        var movements = await _stockMovements.GetByProductAsync(productId, from, to);
        return movements.Select(m => new StockMovementDto
        {
            Id = m.Id,
            ProductId = m.ProductId,
            MovementType = m.MovementType,
            QuantityChange = m.QuantityChange,
            MovementDate = m.MovementDate,
            Notes = m.Notes
        }).ToList();
    }
}
