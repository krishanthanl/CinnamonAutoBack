using Cinnamon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Cinnamon.Infrastructure.Persistence.Configurations;
public class VehicleModelConfiguration : IEntityTypeConfiguration<VehicleModel>
{
    public void Configure(EntityTypeBuilder<VehicleModel> builder)
    {
        builder.ToTable("VehicleModels");
        builder.HasKey(model => model.Id);
        builder.Property(model => model.Name).IsRequired().HasMaxLength(150);
        builder.HasQueryFilter(model => !model.IsDeleted && !model.Brand!.IsDeleted);
        builder.HasOne(model => model.Brand).WithMany(brand => brand.Models)
            .HasForeignKey(model => model.BrandId).OnDelete(DeleteBehavior.Restrict);
        builder.HasIndex(model => new { model.BrandId, model.Name, model.Year });
    }
}
