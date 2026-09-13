using System.Text.Json;
using Cinnamon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Cinnamon.Infrastructure.Persistence.Seed;

/// <summary>
/// One-time migration of the legacy Cosmos-era src/data/parts.json (flat Part records with a
/// string category) into the new normalized Category/Product/CurrentStock/StockMovement schema.
/// Only runs when the Products table is empty, so it is safe to leave wired up permanently.
/// </summary>
public static class DbSeeder
{
    private const int DefaultReorderLevel = 5;
    private const int DefaultReorderAmount = 10;
    private const int DefaultInStockQuantity = 10;

    public static async Task SeedFromLegacyJsonAsync(
        AppDbContext context,
        string legacyPartsJsonPath,
        string sourceImagesFolder,
        string destImagesFolder,
        ILogger logger)
    {
        await context.Database.MigrateAsync();

        if (await context.Products.AnyAsync())
        {
            return;
        }

        if (!File.Exists(legacyPartsJsonPath))
        {
            logger.LogInformation("No legacy parts.json found at {Path}; skipping seed.", legacyPartsJsonPath);
            return;
        }

        List<LegacyPartJson>? legacyParts;
        try
        {
            var json = await File.ReadAllTextAsync(legacyPartsJsonPath);
            legacyParts = JsonSerializer.Deserialize<List<LegacyPartJson>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to read/parse legacy parts.json at {Path}", legacyPartsJsonPath);
            return;
        }

        if (legacyParts is null || legacyParts.Count == 0)
        {
            return;
        }

        var categoryNames = legacyParts.Select(p => p.Category).Distinct(StringComparer.OrdinalIgnoreCase);
        var categoriesByName = new Dictionary<string, Category>(StringComparer.OrdinalIgnoreCase);
        foreach (var name in categoryNames)
        {
            var category = new Category { Name = name };
            categoriesByName[name] = category;
            context.Categories.Add(category);
        }
        await context.SaveChangesAsync();

        var now = DateTime.UtcNow;
        foreach (var legacyPart in legacyParts)
        {
            var category = categoriesByName[legacyPart.Category];
            var quantity = legacyPart.InStock == true ? DefaultInStockQuantity : 0;

            var product = new Product
            {
                Name = legacyPart.Name,
                CategoryId = category.Id,
                ImageSrc = legacyPart.Image,
                Description = legacyPart.Description,
                ReorderLevel = DefaultReorderLevel,
                ReorderAmount = DefaultReorderAmount
            };
            context.Products.Add(product);
            await context.SaveChangesAsync(); // assigns product.Id

            context.CurrentStocks.Add(new CurrentStock { ProductId = product.Id, Quantity = quantity, LastUpdated = now });

            if (quantity > 0)
            {
                context.StockMovements.Add(new StockMovement
                {
                    ProductId = product.Id,
                    MovementType = StockMovementType.AdminAdjustment,
                    QuantityChange = quantity,
                    MovementDate = now,
                    Notes = "Migrated from legacy parts.json"
                });
            }
        }
        await context.SaveChangesAsync();
        logger.LogInformation("Seeded {Count} products from legacy parts.json into {CategoryCount} categories.", legacyParts.Count, categoriesByName.Count);

        CopyImages(sourceImagesFolder, destImagesFolder, logger);
    }

    private static void CopyImages(string sourceImagesFolder, string destImagesFolder, ILogger logger)
    {
        if (!Directory.Exists(sourceImagesFolder))
        {
            return;
        }

        try
        {
            if (!Directory.Exists(destImagesFolder))
            {
                Directory.CreateDirectory(destImagesFolder);
            }

            foreach (var file in Directory.GetFiles(sourceImagesFolder))
            {
                var destFile = Path.Combine(destImagesFolder, Path.GetFileName(file));
                if (!File.Exists(destFile))
                {
                    File.Copy(file, destFile);
                }
            }
            logger.LogInformation("Copied legacy part images from {Source} to {Dest}.", sourceImagesFolder, destImagesFolder);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error copying legacy image assets from {Source} to {Dest}.", sourceImagesFolder, destImagesFolder);
        }
    }
}
