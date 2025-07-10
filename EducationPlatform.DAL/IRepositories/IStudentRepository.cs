using EducationPlatform.DAL.Models.UserModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.IRepositories
{
    public interface IStudentRepository : IRepository<ApplicationUser>
    {
        Task<IEnumerable<ApplicationUser>> GetStudentsByCourseIdAsync(int courseId);
        Task<ApplicationUser?> GetStudentWithCoursesAsync(string studentId);
        Task<IEnumerable<ApplicationUser>> GetAllStudentsAsync();
        Task<ApplicationUser?> GetStudentByIdAsync(string id);
        Task<bool> CanStudentEnrollAsync(string studentId, int courseId);
        Task<ApplicationUser?> CreateAsync(ApplicationUser student);
    }
}
