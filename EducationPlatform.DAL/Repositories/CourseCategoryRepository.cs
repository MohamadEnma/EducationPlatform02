using EducationPlatform.DAL.Data;
using EducationPlatform.DAL.IRepositories;
using Microsoft.EntityFrameworkCore;

namespace EducationPlatform.DAL.Repositories
{
    public class CourseCategoryRepository : Repository<CourseCategory>, ICourseCategoryRepository
    {
        private new readonly EducationDbContext _context;

        public CourseCategoryRepository(EducationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CourseCategory>> GetAllActiveAsync()
        {
            return await _context.Categories
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CourseCategory?> GetBySlugAsync(string slug)
        {
            return await _context.Categories
                .Where(c => c.Slug == slug && c.IsActive && !c.IsDeleted)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<CourseCategory>> GetCategoriesWithCoursesAsync()
        {
            return await _context.Categories
                .Include(c => c.Courses.Where(course => !course.IsDeleted))
                .Where(c => c.IsActive && !c.IsDeleted)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }
    }
}