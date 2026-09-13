using Cinnamon.Application.DTOs;
namespace Cinnamon.Application.Interfaces.Services;
public interface IBrandService
{
    Task<List<BrandDto>> SearchAsync(string? search);
    Task<BrandDto> GetByIdAsync(int id);
    Task<BrandDto> CreateAsync(SaveBrandDto dto);
    Task<BrandDto> UpdateAsync(int id, SaveBrandDto dto);
    Task DeleteAsync(int id);
}
