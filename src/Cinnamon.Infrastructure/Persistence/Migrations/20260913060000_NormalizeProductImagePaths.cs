using Cinnamon.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cinnamon.Infrastructure.Persistence.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("20260913060000_NormalizeProductImagePaths")]
public class NormalizeProductImagePaths : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE Products SET ImageSrc = '/uploads' + ImageSrc " +
            "WHERE ImageSrc LIKE '/parts/product-%'");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            "UPDATE Products SET ImageSrc = SUBSTRING(ImageSrc, 9, LEN(ImageSrc)) " +
            "WHERE ImageSrc LIKE '/uploads/parts/product-%'");
    }
}
