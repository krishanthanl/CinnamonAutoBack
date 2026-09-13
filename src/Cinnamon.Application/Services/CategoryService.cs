using Cinnamon.Application.Common;
using Cinnamon.Application.DTOs;
using Cinnamon.Application.Interfaces;
using Cinnamon.Application.Interfaces.Repositories;
using Cinnamon.Application.Interfaces.Services;
using Cinnamon.Domain.Entities;

namespace Cinnamon.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _categories;
    private readonly IUnitOfWork _unitOfWork;

    public CategoryService(ICategoryRepository categories, IUnitOfWork unitOfWork)
    {
        _categories = categories;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<CategoryDto>> GetAllAsync()
    {
        var categories = await _categories.GetAllAsync();
        return categories.OrderBy(category => category.Name).Select(ToDto).ToList();
    }

    public async Task<CategoryDto> GetByIdAsync(int id)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);
        return ToDto(category);
    }

    public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException("Category name is required.");
        }

        if (dto.ParentId.HasValue && !await _categories.ExistsAsync(dto.ParentId.Value))
        {
            throw new NotFoundException(nameof(Category), dto.ParentId.Value);
        }

        var category = new Category
        {
            Name = dto.Name.Trim(),
            ParentId = dto.ParentId,
            ImageSrc = dto.ImageSrc
        };

        await _categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task<CategoryDto> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);

        if (string.IsNullOrWhiteSpace(dto.Name))
        {
            throw new ValidationException("Category name is required.");
        }

        if (dto.ParentId == id)
        {
            throw new ValidationException("A category cannot be its own parent.");
        }

        if (dto.ParentId.HasValue && !await _categories.ExistsAsync(dto.ParentId.Value))
        {
            throw new NotFoundException(nameof(Category), dto.ParentId.Value);
        }

        category.Name = dto.Name.Trim();
        category.ParentId = dto.ParentId;
        category.ImageSrc = dto.ImageSrc;

        _categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return ToDto(category);
    }

    public async Task DeleteAsync(int id)
    {
        var category = await _categories.GetByIdAsync(id) ?? throw new NotFoundException(nameof(Category), id);
        _categories.Delete(category);
        await _unitOfWork.SaveChangesAsync();
    }

    private static CategoryDto ToDto(Category category) => new()
    {
        Id = category.Id,
        Name = category.Name,
        ParentId = category.ParentId,
        ImageSrc = category.ImageSrc
    };
}
