using EducationPlatform.BLL.DTOs;
using EducationPlatform.BLL.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.Services
{
    public class CourseService : ICourseService
    {
        public Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            throw new NotImplementedException();
        }
        public Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<CourseDto> CreateCourseAsync(CourseDto courseDto)
        {
            throw new NotImplementedException();
        }
        public Task<CourseDto?> UpdateCourseAsync(int id, CourseDto courseDto)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteCourseAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(string instructorId)
        {
            throw new NotImplementedException();
        }
    }
}
