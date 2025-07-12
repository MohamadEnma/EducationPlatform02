using EducationPlatform.DAL.Models.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.IRepositories
{
    public interface ICourseCaegoryRepository : IRepository<CourseCategory>
    {
        Task<CourseCategory?> GetCategoryByIdAsync(int id);
        Task<IEnumerable<CourseCategory>> GetAllActiveCategoriesAsync();
        Task<IEnumerable<CourseCategory>> GetCategoriesByDisplayOrderAsync();
        Task<CourseCategory?> GetCategoryBySlugAsync(string slug);
        Task<CourseCategory?> GetCategoryWithCoursesAsync(int categoryId);
        Task<IEnumerable<CourseCategory>> GetCategoriesWithCourseCountAsync();
        Task<bool> IsCategorySlugUniqueAsync(string slug, int? excludeCategoryId = null);
        Task<CourseCategory?> CreateCategoryAsync(CourseCategory category);
        Task<CourseCategory> UpdateCategoryAsync(CourseCategory category);
        Task<bool> DeleteCategoryAsync(int categoryId);
        Task<bool> SoftDeleteCategoryAsync(int categoryId, string deletedBy);
        Task<int> GetCoursesCountByCategoryAsync(int categoryId);
        Task<IEnumerable<CourseCategory>> SearchCategoriesAsync(string searchTerm);
    }
}