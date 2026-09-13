namespace Cinnamon.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? VehicleModelId { get; set; }
    public string? ImageSrc { get; set; }
    public required string Description { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderAmount { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    public Category? Category { get; set; }
    public Brand? Brand { get; set; }
    public VehicleModel? VehicleModel { get; set; }
    public CurrentStock? CurrentStock { get; set; }
    public ICollection<StockMovement> StockMovements { get; set; } = new List<StockMovement>();
}
