using Cinnamon.Api.Middleware;
using Cinnamon.Application;
using Cinnamon.Infrastructure;
using Cinnamon.Infrastructure.Persistence;
using Cinnamon.Infrastructure.Persistence.Seed;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();

var webRoot = builder.Environment.WebRootPath ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
var partsImagesFolder = Path.Combine(webRoot, "parts");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration, partsImagesFolder);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();
Directory.CreateDirectory(partsImagesFolder);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(partsImagesFolder),
    RequestPath = "/uploads/parts"
});
app.UseCors("AllowAll");
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseAuthorization();
app.MapControllers();

if (builder.Configuration.GetValue<bool>("Database:SeedFromLegacyJson"))
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var legacyJsonPath = builder.Configuration["Database:LegacyPartsJsonPath"]
            ?? Path.Combine(builder.Environment.ContentRootPath, "..", "..", "..", "..", "cinnamon-front", "src", "data", "parts.json");
        var legacyImagesPath = builder.Configuration["Database:LegacyImagesPath"]
            ?? Path.Combine(builder.Environment.ContentRootPath, "..", "..", "..", "..", "cinnamon-front", "public", "parts");

        await DbSeeder.SeedFromLegacyJsonAsync(context, legacyJsonPath, legacyImagesPath, partsImagesFolder, logger);
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "An error occurred while migrating/seeding the SQL Server database.");
    }
}

app.Run();
