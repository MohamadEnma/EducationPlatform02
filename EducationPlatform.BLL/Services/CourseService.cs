using EducationPlatform.BLL.DTOs;
using EducationPlatform.BLL.IServices;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepository;
        public CourseService(ICourseRepository courseRepository)
        {
            _courseRepository = courseRepository ?? throw new ArgumentNullException(nameof(courseRepository));
        }
        public async Task<IEnumerable<CourseDto>> GetAllCoursesAsync()
        {
            try
            {
                var courses = await _courseRepository.GetAllCoursesAsync();
                return courses.Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    IsPublished = c.IsPublished,
                    
                    Price = c.Price,
                    IsFree = c.IsFree,
                    InstructorId = c.InstructorId,
                    
                    CreatedAt = c.CreatedAt
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while retrieving courses.", ex);
            }
        }
        public Task<CourseDto?> GetCourseByIdAsync(int id)
        {
            try
            {
                var course = _courseRepository.GetCourseByIdAsync(id);
                return course.ContinueWith(task =>
                {
                    if (task.Result == null)
                    {
                        return null; // Course not found
                    }
                    return new CourseDto
                    {
                        CourseId = task.Result.CourseId,
                        Title = task.Result.Title,
                        Description = task.Result.Description,
                        IsPublished = task.Result.IsPublished,
                        Price = task.Result.Price,
                        IsFree = task.Result.IsFree,
                        InstructorId = task.Result.InstructorId,
                        CreatedAt = task.Result.CreatedAt
                    };
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException($"An error occurred while retrieving the course with ID {id}.", ex);
            }
        }
        public async Task<CourseDto> CreateCourseAsync(CourseDto courseDto)
        {
            try
            {
                if (courseDto == null)
                {
                    throw new ArgumentNullException(nameof(courseDto), "CourseDto cannot be null.");
                }
                var course = new Course
                {
                    Title = courseDto.Title,
                    Description = courseDto.Description,
                    IsPublished = courseDto.IsPublished,
                    Price = courseDto.Price,
                    IsFree = courseDto.IsFree,
                    InstructorId = courseDto.InstructorId,
                    CreatedAt = DateTime.UtcNow,
                    LastModifiedAt = DateTime.UtcNow
                };
                await _courseRepository.AddCourseAsync(course); // Fix: Ensure AddCourseAsync is awaited properly.  
                return new CourseDto
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    Description = course.Description,
                    IsPublished = course.IsPublished,
                    Price = course.Price,
                    IsFree = course.IsFree,
                    InstructorId = course.InstructorId,
                    CreatedAt = course.CreatedAt
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed  
                throw new ApplicationException("An error occurred while creating the course.", ex);
            }
        }
        public Task<CourseDto?> UpdateCourseAsync(int id, CourseDto courseDto)
        {
            try
            {
                if (courseDto == null)
                {
                    throw new ArgumentNullException(nameof(courseDto), "CourseDto cannot be null.");
                }
                var course = _courseRepository.GetCourseByIdAsync(id);
                return course.ContinueWith(task =>
                {
                    if (task.Result == null)
                    {
                        return null; // Course not found
                    }
                    task.Result.Title = courseDto.Title;
                    task.Result.Description = courseDto.Description;
                    task.Result.IsPublished = courseDto.IsPublished;
                    task.Result.Price = courseDto.Price;
                    task.Result.IsFree = courseDto.IsFree;
                    task.Result.LastModifiedAt = DateTime.UtcNow;
                    _courseRepository.UpdateCourse(task.Result); // Ensure UpdateCourse is called properly
                    return new CourseDto
                    {
                        CourseId = task.Result.CourseId,
                        Title = task.Result.Title,
                        Description = task.Result.Description,
                        IsPublished = task.Result.IsPublished,
                        Price = task.Result.Price,
                        IsFree = task.Result.IsFree,
                        InstructorId = task.Result.InstructorId,
                        CreatedAt = task.Result.CreatedAt
                    };
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException($"An error occurred while updating the course with ID {id}.", ex);
            }
        }
        public Task<bool> DeleteCourseAsync(int id)
        {
            try
            {
                var course = _courseRepository.GetCourseByIdAsync(id);
                return course.ContinueWith(task =>
                {
                    if (task.Result == null)
                    {
                        return false; // Course not found
                    }
                    _courseRepository.DeleteCourse(task.Result); // Ensure DeleteCourse is called properly
                    return true;
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException($"An error occurred while deleting the course with ID {id}.", ex);
            }
        }
        public Task<IEnumerable<CourseDto>> GetCoursesByInstructorAsync(string instructorId)
        {
            try
            {
                var courses = _courseRepository.GetCoursesByInstructorAsync(instructorId);
                return courses.ContinueWith(task =>
                {
                    return task.Result.Select(c => new CourseDto
                    {
                        CourseId = c.CourseId,
                        Title = c.Title,
                        Description = c.Description,
                        IsPublished = c.IsPublished,
                        Price = c.Price,
                        IsFree = c.IsFree,
                        InstructorId = c.InstructorId,
                        CreatedAt = c.CreatedAt
                    });
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException($"An error occurred while retrieving courses for instructor {instructorId}.", ex);
            }
        }

        public async Task<IEnumerable<CourseDto>> GetPublishedCoursesAsync()
        {
            try
            {
                var courses = await _courseRepository.GetPublishedCoursesAsync();
                return courses.Select(c => new CourseDto
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    IsPublished = c.IsPublished,
                    Price = c.Price,
                    IsFree = c.IsFree,
                    InstructorId = c.InstructorId,
                    CreatedAt = c.CreatedAt
                });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while retrieving published courses.", ex);
            }
        }

        public async Task<CourseDto?> GetCourseWithDetailsAsync(int courseId)
        {
            try
            {
                var course = await _courseRepository.GetCourseWithDetailsAsync(courseId);
                if (course == null)
                {
                    return null; // Course not found
                }
                return new CourseDto
                {
                    CourseId = course.CourseId,
                    Title = course.Title,
                    Description = course.Description,
                    IsPublished = course.IsPublished,
                    Price = course.Price,
                    IsFree = course.IsFree,
                    InstructorId = course.InstructorId,
                    CreatedAt = course.CreatedAt
                };
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException($"An error occurred while retrieving the course with ID {courseId}.", ex);
            }
        }

        public async Task<int> SaveChangesAsync()
        {
            try
            {
                
                return await _courseRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                throw new ApplicationException("An error occurred while saving changes.", ex);
            }
        }
    }
}
