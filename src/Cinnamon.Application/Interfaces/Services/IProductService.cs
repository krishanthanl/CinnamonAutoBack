using Cinnamon.Application.DTOs;

namespace Cinnamon.Application.Interfaces.Services;

public interface IProductService
{
    Task<List<ProductDto>> GetAllAsync();
    Task<ProductDto> GetByIdAsync(int id);
    Task<List<ProductDto>> GetByCategoryAsync(int categoryId);
    Task<List<ProductDto>> SearchAsync(int? categoryId, string? search);
    Task<ProductDto> CreateAsync(CreateProductDto dto);
    Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto);
    Task DeleteAsync(int id);
}
