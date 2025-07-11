using EducationPlatform.DAL.IRepositories;

namespace EducationPlatform.DAL.IRepositories
{
    public interface ICourseCategoryRepository : IRepository<CourseCategory>
    {
        Task<IEnumerable<CourseCategory>> GetAllActiveAsync();
        Task<CourseCategory?> GetBySlugAsync(string slug);
        Task<IEnumerable<CourseCategory>> GetCategoriesWithCoursesAsync();
    }
}