using Cinnamon.Domain.Entities;
namespace Cinnamon.Application.Interfaces.Repositories;
public interface IVehicleModelRepository
{
    Task<List<VehicleModel>> SearchAsync(int? brandId, string? search);
    Task<VehicleModel?> GetByIdAsync(int id);
    Task AddAsync(VehicleModel model);
    void Update(VehicleModel model);
    void Delete(VehicleModel model);
}
