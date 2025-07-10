using EducationPlatform.DAL.Data;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Repositories
{
    public class StudentRepository : Repository<ApplicationUser>, IStudentRepository
    {
        public StudentRepository(EducationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ApplicationUser>> GetStudentsByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Where(s => s.Courses.Any(c => c.CourseId == courseId) && !s.IsDeleted)
                .Include(s => s.Courses)
                .ToListAsync();

        }

        public async Task<ApplicationUser?> GetStudentWithCoursesAsync(string studentId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
        }
        public async Task<IEnumerable<ApplicationUser>> GetAllStudentsAsync()
        {
            return await _dbSet
                .Where(s => !s.IsDeleted)
                .ToListAsync();
        }
        public async Task<ApplicationUser?> GetStudentByIdAsync(string id)
        {
            return await _dbSet
                .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted);
        }

        public async Task<bool> CanStudentEnrollAsync(string studentId, int courseId)
        {
            var student = await _dbSet
                .Include(s => s.Courses)
                .FirstOrDefaultAsync(s => s.Id == studentId.ToString() && !s.IsDeleted);
            if (student == null)
            {
                return false;
            }
            // Check if the student is already enrolled in the course
            return !student.Courses.Any(c => c.CourseId == courseId);
        }
        public async Task<ApplicationUser?> CreateAsync(ApplicationUser student)
        {
            if (student == null)
            {
                throw new ArgumentNullException(nameof(student));
            }

            await _dbSet.AddAsync(student);
            await _context.SaveChangesAsync();

            return student;
        }

    }
}