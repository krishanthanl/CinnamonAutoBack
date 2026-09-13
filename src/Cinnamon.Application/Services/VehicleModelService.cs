using Cinnamon.Application.Common;
using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Application.Interfaces.Services;
using Cinnamon.Domain.Entities;
namespace Cinnamon.Application.Services;
public class VehicleModelService : IVehicleModelService
{
    private readonly IVehicleModelRepository _models;
    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _unitOfWork;
    public VehicleModelService(IVehicleModelRepository models, IBrandRepository brands, IUnitOfWork unitOfWork) { _models = models; _brands = brands; _unitOfWork = unitOfWork; }
    public async Task<List<VehicleModelDto>> SearchAsync(int? brandId, string? search) => (await _models.SearchAsync(brandId, search)).Select(ToDto).ToList();
    public async Task<VehicleModelDto> GetByIdAsync(int id) => ToDto(await _models.GetByIdAsync(id) ?? throw new NotFoundException(nameof(VehicleModel), id));
    public async Task<VehicleModelDto> CreateAsync(SaveVehicleModelDto dto)
    {
        await ValidateAsync(dto);
        var model = new VehicleModel { Name = dto.Name.Trim(), Year = dto.Year, BrandId = dto.BrandId };
        await _models.AddAsync(model); await _unitOfWork.SaveChangesAsync();
        model = await _models.GetByIdAsync(model.Id) ?? model; return ToDto(model);
    }
    public async Task<VehicleModelDto> UpdateAsync(int id, SaveVehicleModelDto dto)
    {
        await ValidateAsync(dto);
        var model = await _models.GetByIdAsync(id) ?? throw new NotFoundException(nameof(VehicleModel), id);
        model.Name = dto.Name.Trim(); model.Year = dto.Year; model.BrandId = dto.BrandId;
        _models.Update(model); await _unitOfWork.SaveChangesAsync();
        model = await _models.GetByIdAsync(model.Id) ?? model; return ToDto(model);
    }
    public async Task DeleteAsync(int id)
    {
        var model = await _models.GetByIdAsync(id) ?? throw new NotFoundException(nameof(VehicleModel), id);
        _models.Delete(model); await _unitOfWork.SaveChangesAsync();
    }
    private async Task ValidateAsync(SaveVehicleModelDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name)) throw new ValidationException("Model name is required.");
        if (dto.Year < 1886 || dto.Year > DateTime.UtcNow.Year + 2) throw new ValidationException($"Year must be between 1886 and {DateTime.UtcNow.Year + 2}.");
        if (!await _brands.ExistsAsync(dto.BrandId)) throw new NotFoundException(nameof(Brand), dto.BrandId);
    }
    private static VehicleModelDto ToDto(VehicleModel model) => new() { Id = model.Id, Name = model.Name, Year = model.Year, BrandId = model.BrandId, BrandName = model.Brand?.Name ?? string.Empty };
}
