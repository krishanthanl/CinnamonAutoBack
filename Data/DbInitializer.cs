using System.Text.Json;
using cinnamon_back.Data;
using cinnamon_back.Models;

namespace cinnamon_back.Data;

public static class DbInitializer
{
    public static async Task Initialize(AppDbContext context, string partsJsonPath, string sourceImagesFolder, string destImagesFolder)
    {
        // Ensure database and container exist
        await context.Database.EnsureCreatedAsync();

        // Seed data
        if (File.Exists(partsJsonPath) && !context.Parts.Any())
        {
            try
            {
                var json = File.ReadAllText(partsJsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var parts = JsonSerializer.Deserialize<List<Part>>(json, options);

                if (parts != null && parts.Count > 0)
                {
                    await context.Parts.AddRangeAsync(parts);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"Seeded {parts.Count} parts into Cosmos DB.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error seeding database: {ex.Message}");
            }
        }

        // Copy image assets
        if (Directory.Exists(sourceImagesFolder))
        {
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
                Console.WriteLine("Successfully copied image assets to wwwroot/parts");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error copying image assets: {ex.Message}");
            }
        }
    }
}
