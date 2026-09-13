using Cinnamon.Application.Common;
using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Application.Interfaces.Services;
using Cinnamon.Domain.Entities;
namespace Cinnamon.Application.Services;
public class BrandService : IBrandService
{
    private readonly IBrandRepository _brands;
    private readonly IUnitOfWork _unitOfWork;
    public BrandService(IBrandRepository brands, IUnitOfWork unitOfWork) { _brands = brands; _unitOfWork = unitOfWork; }
    public async Task<List<BrandDto>> SearchAsync(string? search) => (await _brands.SearchAsync(search)).Select(ToDto).ToList();
    public async Task<BrandDto> GetByIdAsync(int id) => ToDto(await _brands.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Brand), id));
    public async Task<BrandDto> CreateAsync(SaveBrandDto dto)
    {
        Validate(dto);
        var brand = new Brand { Name = dto.Name.Trim() };
        await _brands.AddAsync(brand); await _unitOfWork.SaveChangesAsync(); return ToDto(brand);
    }
    public async Task<BrandDto> UpdateAsync(int id, SaveBrandDto dto)
    {
        Validate(dto);
        var brand = await _brands.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Brand), id);
        brand.Name = dto.Name.Trim(); _brands.Update(brand); await _unitOfWork.SaveChangesAsync(); return ToDto(brand);
    }
    public async Task DeleteAsync(int id)
    {
        var brand = await _brands.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Brand), id);
        _brands.Delete(brand); await _unitOfWork.SaveChangesAsync();
    }
    private static void Validate(SaveBrandDto dto) { if (string.IsNullOrWhiteSpace(dto.Name)) throw new ValidationException("Brand name is required."); }
    private static BrandDto ToDto(Brand brand) => new() { Id = brand.Id, Name = brand.Name };
}
