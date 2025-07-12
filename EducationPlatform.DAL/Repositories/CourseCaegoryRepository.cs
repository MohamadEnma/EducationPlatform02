using EducationPlatform.DAL.Data;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Repositories
{
    public class CourseCaegoryRepository : Repository<CourseCategory>, ICourseCaegoryRepository
    {
        public CourseCaegoryRepository(EducationDbContext context) : base(context)
        {
        }


        // Specific course category methods
        public async Task<CourseCategory?> GetCategoryByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Courses.Where(course => !course.IsDeleted))
                .FirstOrDefaultAsync(c => c.CourseCategoryId == id && !c.IsDeleted);
        }

        public async Task<IEnumerable<CourseCategory>> GetAllActiveCategoriesAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive && !c.IsDeleted)
                .Include(c => c.Courses.Where(course => course.IsPublished && !course.IsDeleted))
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<CourseCategory>> GetCategoriesByDisplayOrderAsync()
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Courses)
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<CourseCategory?> GetCategoryBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return null;

            return await _dbSet
                .Include(c => c.Courses.Where(course => course.IsPublished && !course.IsDeleted))
                .FirstOrDefaultAsync(c => c.Slug == slug && !c.IsDeleted);
        }

        public async Task<CourseCategory?> GetCategoryWithCoursesAsync(int categoryId)
        {
            return await _dbSet
                .Include(c => c.Courses.Where(course => !course.IsDeleted))
                .ThenInclude(course => course.Instructor)
                .FirstOrDefaultAsync(c => c.CourseCategoryId == categoryId && !c.IsDeleted);
        }

        public async Task<IEnumerable<CourseCategory>> GetCategoriesWithCourseCountAsync()
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Courses.Where(course => course.IsPublished && !course.IsDeleted))
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        public async Task<bool> IsCategorySlugUniqueAsync(string slug, int? excludeCategoryId = null)
        {
            if (string.IsNullOrWhiteSpace(slug))
                return false;

            var query = _dbSet.Where(c => c.Slug == slug && !c.IsDeleted);

            if (excludeCategoryId.HasValue)
            {
                query = query.Where(c => c.CourseCategoryId != excludeCategoryId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<CourseCategory?> CreateCategoryAsync(CourseCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Set audit fields
            category.CreatedUtc = DateTime.UtcNow;
            category.CreatedBy = "MohamadEnma"; // You might want to pass this as a parameter
            category.LastModifiedAt = DateTime.Now;
            category.IsDeleted = false;

            await _dbSet.AddAsync(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<CourseCategory> UpdateCategoryAsync(CourseCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category));

            // Update audit fields
            category.ModifiedUtc = DateTime.UtcNow;
            category.ModifiedBy = "MohamadEnma"; // You might want to pass this as a parameter
            category.LastModifiedAt = DateTime.Now;

            _dbSet.Update(category);
            await _context.SaveChangesAsync();

            return category;
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category == null || category.IsDeleted)
                return false;

            // Check if category has courses
            var hasActiveCourses = await _context.Courses
                .AnyAsync(c => c.CategoryId == categoryId && !c.IsDeleted);

            if (hasActiveCourses)
                throw new InvalidOperationException("Cannot delete category that contains courses.");

            // Hard delete if no courses
            _dbSet.Remove(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> SoftDeleteCategoryAsync(int categoryId, string deletedBy)
        {
            var category = await _dbSet.FindAsync(categoryId);
            if (category == null || category.IsDeleted)
                return false;

            // Soft delete
            category.IsDeleted = true;
            category.DeletedAt = DateTime.UtcNow;
            category.DeletedBy = deletedBy;

            _dbSet.Update(category);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<int> GetCoursesCountByCategoryAsync(int categoryId)
        {
            return await _context.Courses
                .CountAsync(c => c.CategoryId == categoryId && c.IsPublished && !c.IsDeleted);
        }

        public async Task<IEnumerable<CourseCategory>> SearchCategoriesAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return await GetAllActiveCategoriesAsync();

            return await _dbSet
                .Where(c => !c.IsDeleted && c.IsActive &&
                           (c.Name.Contains(searchTerm) ||
                            c.Description.Contains(searchTerm) ||
                            c.Keywords.Contains(searchTerm)))
                .Include(c => c.Courses.Where(course => course.IsPublished && !course.IsDeleted))
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }
    }
}