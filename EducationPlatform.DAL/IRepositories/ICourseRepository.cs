using EducationPlatform.DAL.Models.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.IRepositories
{
    public interface ICourseRepository
    {
        Task<Course?> GetCourseByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> FindCoursesAsync(Func<Course, bool> predicate);
        Task AddCourseAsync(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
    }
}
