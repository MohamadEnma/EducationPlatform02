using EducationPlatform.DAL.Data;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Repositories
{
    public class CourseRepository : Repository<Course>, ICourseRepository
    {
        public CourseRepository(EducationDbContext context) : base(context)
        {
        }
        public async Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId)
        {
            return await _dbSet
                .Where(c => c.InstructorId == instructorId && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetPublishedCoursesAsync()
        {
            return await _dbSet
                .Where(c => c.IsPublished && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<Course?> GetCourseWithDetailsAsync(int courseId)
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.CourseId == courseId && !c.IsDeleted);
        }
        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .FirstOrDefaultAsync(c => c.CourseId == id && !c.IsDeleted);
        }
        public async Task<IEnumerable<Course>> GetAllCoursesAsync()
        {
            return await _dbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> FindCoursesAsync(Func<Course, bool> predicate)
        {
            // Note: This loads all courses into memory first, then applies the predicate
            // For better performance with large datasets, consider using Expression<Func<Course, bool>>
            var allCourses = await _dbSet
                .Where(c => !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();

            return allCourses.Where(predicate);
        }
        public async Task AddCourseAsync(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            // Set audit fields
            course.CreatedUtc = DateTime.UtcNow;
            course.CreatedBy = "MohamadEnma"; // You might want to pass this as a parameter
            course.CreatedAt = DateTime.Now;
            course.LastModifiedAt = DateTime.Now;
            course.IsDeleted = false;

            await _dbSet.AddAsync(course);
        }
        public void UpdateCourse(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            // Update audit fields
            course.ModifiedUtc = DateTime.UtcNow;
            course.ModifiedBy = "MohamadEnma"; // You might want to pass this as a parameter
            course.LastModifiedAt = DateTime.Now;

            _dbSet.Update(course);
        }
        public void DeleteCourse(Course course)
        {
            if (course == null)
                throw new ArgumentNullException(nameof(course));

            // Soft delete
            course.IsDeleted = true;
            course.DeletedAt = DateTime.UtcNow;
            course.DeletedBy = "MohamadEnma"; // You might want to pass this as a parameter

            _dbSet.Update(course);
        }
        public async Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId)
        {
            return await _dbSet
                .Where(c => c.CategoryId == categoryId && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByStatusAsync(CourseStatus status)
        {
            return await _dbSet
                .Where(c => c.Status == status && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByDifficultyAsync(CourseDifficulty difficulty)
        {
            return await _dbSet
                .Where(c => c.Difficulty == difficulty && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByTitleAsync(string title)
        {
            return await _dbSet
                .Where(c => c.Title.Contains(title) && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByKeywordAsync(string keyword)
        {
            return await _dbSet
                .Where(c => c.Title.Contains(keyword) || c.Description.Contains(keyword) && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByEnrollmentCountAsync(int minEnrollments)
        {
            return await _dbSet
                .Where(c => c.Enrollments.Count >= minEnrollments && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByCreationDateAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(c => c.CreatedUtc >= startDate && c.CreatedUtc <= endDate && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<IEnumerable<Course>> GetCoursesByLastModifiedDateAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(c => c.ModifiedUtc >= startDate && c.ModifiedUtc <= endDate && !c.IsDeleted)
                .Include(c => c.Instructor)
                .Include(c => c.Category)
                .ToListAsync();
        }
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

    }
}