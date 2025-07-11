﻿using EducationPlatform.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.IServices
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto?> GetCourseByIdAsync(int id);
        Task<CourseDto> CreateCourseAsync(CourseDto courseDto);
        Task<CourseDto?> UpdateCourseAsync(int id, CourseDto courseDto);
        Task<bool> DeleteCourseAsync(int id);
        Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(string instructorId);
        Task<int> SaveChangesAsync();
    }
}
