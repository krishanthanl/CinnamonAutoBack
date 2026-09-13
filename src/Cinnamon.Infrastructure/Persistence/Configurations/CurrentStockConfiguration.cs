using Cinnamon.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Cinnamon.Infrastructure.Persistence.Configurations;

public class CurrentStockConfiguration : IEntityTypeConfiguration<CurrentStock>
{
    public void Configure(EntityTypeBuilder<CurrentStock> builder)
    {
        builder.ToTable("CurrentStocks");
        builder.HasKey(s => s.ProductId);
        builder.HasQueryFilter(s => !s.Product!.IsDeleted && !s.Product.Category!.IsDeleted);

        builder.HasOne(s => s.Product)
            .WithOne(p => p.CurrentStock)
            .HasForeignKey<CurrentStock>(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
