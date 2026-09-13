using Cinnamon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinnamon.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        builder.Property(p => p.Description).IsRequired().HasMaxLength(2000);
        builder.Property(p => p.ImageSrc).HasMaxLength(500);
        builder.Property(p => p.CostPrice).HasPrecision(18, 2);
        builder.Property(p => p.SellingPrice).HasPrecision(18, 2);
        builder.HasQueryFilter(p => !p.IsDeleted && !p.Category!.IsDeleted);

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(p => p.VehicleModel)
            .WithMany(m => m.Products)
            .HasForeignKey(p => p.VehicleModelId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
