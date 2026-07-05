namespace cinnamon_back.Models;

public class Part
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required string Image { get; set; }
    public required string Description { get; set; }
    public bool? InStock { get; set; }
}
