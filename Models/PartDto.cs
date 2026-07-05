namespace cinnamon_back.Models;

public class PartDto
{
    public required string Name { get; set; }
    public required string Category { get; set; }
    public required string Description { get; set; }
    public required string ImageBase64 { get; set; }
    public string? ImageExtension { get; set; }
    public bool InStock { get; set; }
}
