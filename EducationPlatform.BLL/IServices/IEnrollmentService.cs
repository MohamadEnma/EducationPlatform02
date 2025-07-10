using EducationPlatform.DAL.Models.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.IServices
{
    public interface IEnrollmentService
    {
        Task<Enrollment> EnrollStudentAsync(string studentId, int courseId);
        Task<Enrollment> UpdateGradeAsync(int enrollmentId, decimal grade);
        Task<bool> DropEnrollmentAsync(int enrollmentId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId);
        Task<Enrollment?> GetEnrollmentAsync(string studentId, int courseId);
        Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment);
        Task<Enrollment> UpdateEnrollmentAsync(Enrollment enrollment);
        Task<bool> DeleteEnrollmentAsync(int enrollmentId);
        Task<int> GetCourseEnrollmentCountAsync(int courseId);
        Task<bool> CanStudentEnrollAsync(string studentId, int courseId);
        Task<bool> IsStudentEnrolledInCourseAsync(string studentId, int courseId);
        Task<bool> IsEnrollmentActiveAsync(int enrollmentId);
        Task<Enrollment?> GetEnrollmentByIdAsync(int enrollmentId);
    }
}
