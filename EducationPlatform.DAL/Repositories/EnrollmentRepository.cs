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
    public class EnrollmentRepository : Repository<Enrollment> , IEnrollmentRepository
    {
        public EnrollmentRepository(EducationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId)
        {
            return await _dbSet
                .Where(e => e.StudentId == studentId)
                .ToListAsync();
        }
        public async Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            return await _dbSet
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }
        public async Task<Enrollment?> GetEnrollmentAsync(string studentId, int courseId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
        }
        public Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            return Task.Run(() =>
            {
                _dbSet.Add(enrollment);
                _context.SaveChanges();
                return enrollment;
            });
        }
        public async Task<bool> DeleteEnrollmentAsync(string studentId, int courseId)
        {
            var enrollment = await _dbSet.FirstOrDefaultAsync(e => e.StudentId == studentId && e.CourseId == courseId);
            if (enrollment != null)
            {
                _dbSet.Remove(enrollment);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<Enrollment> UpdateAsync(Enrollment enrollment)
        {
            _dbSet.Update(enrollment);
            await _context.SaveChangesAsync();
            return enrollment;
        }

        public Task<int> GetCourseEnrollmentCountAsync(int courseId)
        {
            return _dbSet
                .CountAsync(e => e.CourseId == courseId && e.IsActive);
        }

        public async Task<Enrollment?> GetEnrollmentByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }
    }
}
