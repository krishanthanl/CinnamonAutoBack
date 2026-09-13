namespace Cinnamon.Application.DTOs;

public class CategoryDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int? ParentId { get; set; }
    public string? ImageSrc { get; set; }
}

public class CreateCategoryDto
{
    public required string Name { get; set; }
    public int? ParentId { get; set; }
    public string? ImageSrc { get; set; }
}

public class UpdateCategoryDto
{
    public required string Name { get; set; }
    public int? ParentId { get; set; }
    public string? ImageSrc { get; set; }
}
