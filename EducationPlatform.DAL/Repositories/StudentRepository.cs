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

       
    }
}
