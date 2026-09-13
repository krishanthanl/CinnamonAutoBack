namespace Cinnamon.Domain.Entities;

public class VehicleModel
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public int BrandId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public Brand? Brand { get; set; }
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
