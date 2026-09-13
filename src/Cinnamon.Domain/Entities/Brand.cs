namespace Cinnamon.Domain.Entities;

public class Brand
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
    public ICollection<VehicleModel> Models { get; set; } = new List<VehicleModel>();
    public ICollection<Product> Products { get; set; } = new List<Product>();
}
