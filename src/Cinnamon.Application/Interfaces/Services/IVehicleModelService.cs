using Cinnamon.Application.DTOs;
namespace Cinnamon.Application.Interfaces.Services;
public interface IVehicleModelService
{
    Task<List<VehicleModelDto>> SearchAsync(int? brandId, string? search);
    Task<VehicleModelDto> GetByIdAsync(int id);
    Task<VehicleModelDto> CreateAsync(SaveVehicleModelDto dto);
    Task<VehicleModelDto> UpdateAsync(int id, SaveVehicleModelDto dto);
    Task DeleteAsync(int id);
}
