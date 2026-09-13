namespace Cinnamon.Application.DTOs;

public class VehicleModelDto
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public int Year { get; set; }
    public int BrandId { get; set; }
    public required string BrandName { get; set; }
}
public class SaveVehicleModelDto
{
    public required string Name { get; set; }
    public int Year { get; set; }
    public int BrandId { get; set; }
}
