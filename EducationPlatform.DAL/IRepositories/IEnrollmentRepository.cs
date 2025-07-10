using EducationPlatform.DAL.Models.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.IRepositories
{
    public interface IEnrollmentRepository
    {
        // Define methods for enrollment management
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId);
        Task<Enrollment?> GetEnrollmentAsync(string studentId, int courseId);
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        Task<bool> DeleteEnrollmentAsync(string studentId, int courseId);
        Task<Enrollment> UpdateAsync(Enrollment enrollment);
        Task<int> GetCourseEnrollmentCountAsync(int courseId);
        Task<Enrollment> GetEnrollmentByIdAsync(int enrollmentId);

    }
}
