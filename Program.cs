using Microsoft.EntityFrameworkCore;
using cinnamon_back.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Configure Cosmos DB
builder.Services.AddDbContext<AppDbContext>(options =>
{
    var connectionString = builder.Configuration["CosmosDb:ConnectionString"] ?? "AccountEndpoint=https://localhost:8081/;";
    var databaseName = builder.Configuration["CosmosDb:DatabaseName"] ?? "CinnamonDb";
    options.UseCosmos(connectionString, databaseName);
});

// Configure CORS
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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseStaticFiles();

app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

if (builder.Configuration.GetValue<bool>("CosmosDb:EnsureCreated"))
{
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();
            await context.Database.EnsureCreatedAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while creating the Cosmos DB database or containers.");
        }
    }
}

app.Run();
