using EducationPlatform.BLL.DTOs;
using EducationPlatform.BLL.IServices;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.UserModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.Services
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ILogger<StudentService> _logger;

        public StudentService(
            IStudentRepository studentRepository,
            IEnrollmentRepository enrollmentRepository,
            ILogger<StudentService> logger)
        {
            _studentRepository = studentRepository;
            _enrollmentRepository = enrollmentRepository;
            _logger = logger;
        }
        public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            try
            {
                var students = await _studentRepository.GetAllAsync();
                return students.Select(s => new StudentDto
                {
                    Id = s.Id ?? string.Empty, // Ensure null safety  
                    FirstName = s.FirstName ?? string.Empty, // Ensure null safety  
                    LastName = s.LastName ?? string.Empty, // Ensure null safety  
                    Email = s.Email ?? string.Empty, // Ensure null safety  
                    DateOfBirth = s.DateOfBirth
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all students.");
                return Enumerable.Empty<StudentDto>(); // Return an empty collection in case of an error  
            }
        }
        public Task<StudentDto?> GetStudentByIdAsync(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                _logger.LogWarning("GetStudentByIdAsync called with null or empty id.");
                return Task.FromResult<StudentDto?>(null);
            }
            return _studentRepository.GetStudentByIdAsync(id)
                .ContinueWith(task =>
                {
                    if (task.Result == null)
                    {
                        _logger.LogInformation("Student with id {Id} not found.", id);
                        return null;
                    }
                    var student = task.Result;
                    return new StudentDto
                    {
                        Id = student.Id ?? string.Empty, // Ensure null safety  
                        FirstName = student.FirstName ?? string.Empty, // Ensure null safety  
                        LastName = student.LastName ?? string.Empty, // Ensure null safety  
                        Email = student.Email ?? string.Empty, // Ensure null safety  
                        DateOfBirth = student.DateOfBirth
                    };
                });

        }
        public async Task<StudentDto> CreateStudentAsync(StudentDto studentDto)
        {
            if (studentDto == null)
            {
                throw new ArgumentNullException(nameof(studentDto), "StudentDto cannot be null.");
            }

            try
            {
                var createdStudent = await _studentRepository.CreateAsync(new ApplicationUser
                {
                    Id = studentDto.Id,
                    FirstName = studentDto.FirstName,
                    LastName = studentDto.LastName,
                    Email = studentDto.Email,
                    DateOfBirth = studentDto.DateOfBirth
                });

                if (createdStudent == null)
                {
                    throw new InvalidOperationException("Failed to create student.");
                }

                return new StudentDto
                {
                    Id = createdStudent.Id ?? string.Empty,
                    FirstName = createdStudent.FirstName ?? string.Empty,
                    LastName = createdStudent.LastName ?? string.Empty,
                    Email = createdStudent.Email ?? string.Empty,
                    DateOfBirth = createdStudent.DateOfBirth
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a student.");
                throw;
            }
        }
        public Task<StudentDto?> UpdateStudentAsync(string id, StudentDto studentDto)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteStudentAsync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanStudentEnrollAsync(string studentId, int courseId)
        {
            throw new NotImplementedException();
        }

        
    }
    
}
