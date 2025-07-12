using EducationPlatform.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.IServices
{
    public interface ICourseCategoryService
    {
        // Basic CRUD operations
        Task<IEnumerable<CourseCategoryDto>> GetAllCategoriesAsync();
        Task<CourseCategoryDto?> GetCategoryByIdAsync(int id);
        Task<CourseCategoryDto> CreateCategoryAsync(CourseCategoryDto categoryDto);
        Task<CourseCategoryDto?> UpdateCategoryAsync(int id, CourseCategoryDto categoryDto);
        Task<bool> DeleteCategoryAsync(int id);
        Task<bool> SoftDeleteCategoryAsync(int id, string deletedBy);

        // Specific category operations
        Task<IEnumerable<CourseCategoryDto>> GetActiveCategoriesAsync();
        Task<IEnumerable<CourseCategoryDto>> GetCategoriesByDisplayOrderAsync();
        Task<CourseCategoryDto?> GetCategoryBySlugAsync(string slug);
        Task<CourseCategoryDto?> GetCategoryWithCoursesAsync(int categoryId);
        Task<IEnumerable<CourseCategoryDto>> GetCategoriesWithCourseCountAsync();
        Task<IEnumerable<CourseCategoryDto>> SearchCategoriesAsync(string searchTerm);

        // Business logic operations
        Task<bool> IsCategorySlugUniqueAsync(string slug, int? excludeCategoryId = null);
        Task<int> GetCoursesCountByCategoryAsync(int categoryId);
        Task<bool> CanDeleteCategoryAsync(int categoryId);
        Task<string> GenerateSlugAsync(string categoryName);
        Task<IEnumerable<CourseCategoryDto>> GetCategoriesForDropdownAsync();
    }
}
