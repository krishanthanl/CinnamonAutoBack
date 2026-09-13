using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.DTOs;

public class StockMovementDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int QuantityChange { get; set; }
    public DateTime MovementDate { get; set; }
    public string? Notes { get; set; }
}

public class CreateStockMovementDto
{
    public int ProductId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int QuantityChange { get; set; }
    public DateTime? MovementDate { get; set; }
    public string? Notes { get; set; }
}

public class ProductStockAtDateDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public DateTime AsOfDate { get; set; }
    public int Quantity { get; set; }
}
