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
            throw new NotImplementedException();

        }

        public Task AddCourseAsync(Course course)
        {
            throw new NotImplementedException();

        }

        public void UpdateCourse(Course course)
        {
            throw new NotImplementedException();
        }

        public void DeleteCourse(Course course)
        {
            throw new NotImplementedException();
        }
    }
}
