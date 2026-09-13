namespace Cinnamon.Infrastructure.Persistence.Seed;

/// <summary>Shape of the old Cosmos-era src/data/parts.json records, used only for one-time seeding.</summary>
public class LegacyPartJson
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool? InStock { get; set; }
}
