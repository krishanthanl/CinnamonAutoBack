namespace Cinnamon.Application.DTOs;

public class ProductDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public int? BrandId { get; set; }
    public string? BrandName { get; set; }
    public int? VehicleModelId { get; set; }
    public string? VehicleModelName { get; set; }
    public int? VehicleModelYear { get; set; }
    public string? ImageSrc { get; set; }
    public required string Description { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderAmount { get; set; }
    public int CurrentQuantity { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public bool InStock => CurrentQuantity > 0;
}

public class CreateProductDto
{
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? VehicleModelId { get; set; }
    public required string Description { get; set; }
    public required string ImageBase64 { get; set; }
    public string? ImageExtension { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderAmount { get; set; }
    public int InitialQuantity { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
}

public class UpdateProductDto
{
    public required string Name { get; set; }
    public int CategoryId { get; set; }
    public int? BrandId { get; set; }
    public int? VehicleModelId { get; set; }
    public required string Description { get; set; }
    public int ReorderLevel { get; set; }
    public int ReorderAmount { get; set; }
    public int CurrentQuantity { get; set; }
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public string? ImageBase64 { get; set; }
    public string? ImageExtension { get; set; }
}
