namespace Cinnamon.Domain.Entities;

/// <summary>
/// A signed quantity change for a product. Sales are recorded as negative
/// QuantityChange; admin stock updates can be positive (restock) or negative (correction).
/// Summing QuantityChange for a product up to a given date yields the stock level on that date.
/// </summary>
public class StockMovement
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public StockMovementType MovementType { get; set; }
    public int QuantityChange { get; set; }
    public DateTime MovementDate { get; set; }
    public string? Notes { get; set; }

    public Product? Product { get; set; }
}
