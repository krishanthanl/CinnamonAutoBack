namespace Cinnamon.Domain.Entities;

public class CurrentStock
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime LastUpdated { get; set; }

    public Product? Product { get; set; }
}
