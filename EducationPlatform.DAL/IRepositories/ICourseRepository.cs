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
        Task<IEnumerable<Course>> GetCoursesByInstructorAsync(string instructorId);
        Task<IEnumerable<Course>> GetPublishedCoursesAsync();
        Task<Course?> GetCourseWithDetailsAsync(int courseId);
        Task<Course?> GetCourseByIdAsync(int id);
        Task<IEnumerable<Course>> GetAllCoursesAsync();
        Task<IEnumerable<Course>> FindCoursesAsync(Func<Course, bool> predicate);
        Task AddCourseAsync(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(Course course);
        Task<IEnumerable<Course>> GetCoursesByCategoryAsync(int categoryId);
        Task<IEnumerable<Course>> GetCoursesByStatusAsync(CourseStatus status);
        Task<IEnumerable<Course>> GetCoursesByDifficultyAsync(CourseDifficulty difficulty);
        Task<IEnumerable<Course>> GetCoursesByTitleAsync(string title);
        Task<IEnumerable<Course>> GetCoursesByKeywordAsync(string keyword);
        Task<IEnumerable<Course>> GetCoursesByEnrollmentCountAsync(int minEnrollments);
        Task<IEnumerable<Course>> GetCoursesByCreationDateAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Course>> GetCoursesByLastModifiedDateAsync(DateTime startDate, DateTime endDate);
        Task<int> SaveChangesAsync();

    }
}
