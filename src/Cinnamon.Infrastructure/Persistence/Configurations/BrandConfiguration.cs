using Cinnamon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Cinnamon.Infrastructure.Persistence.Configurations;
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.ToTable("Brands");
        builder.HasKey(brand => brand.Id);
        builder.Property(brand => brand.Name).IsRequired().HasMaxLength(150);
        builder.HasQueryFilter(brand => !brand.IsDeleted);
    }
}
