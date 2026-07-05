using Microsoft.EntityFrameworkCore;
using cinnamon_back.Models;

namespace cinnamon_back.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Part> Parts => Set<Part>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Part>(entity =>
        {
            entity.ToContainer("Part");
            entity.HasPartitionKey(p => p.Id);
            entity.HasNoDiscriminator();
            entity.Property(p => p.Id).ToJsonProperty("id");
            entity.Property(p => p.Name).ToJsonProperty("name");
            entity.Property(p => p.Category).ToJsonProperty("category");
            entity.Property(p => p.Image).ToJsonProperty("image");
            entity.Property(p => p.Description).ToJsonProperty("description");
            entity.Property(p => p.InStock).ToJsonProperty("inStock");
        });
    }
}
