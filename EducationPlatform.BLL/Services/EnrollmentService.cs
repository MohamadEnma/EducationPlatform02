using EducationPlatform.BLL.Exceptions;
using EducationPlatform.BLL.IServices;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.Services
{
    public class EnrollmentService : IEnrollmentService
    {
        
    private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentService _studentService;
        private readonly ILogger<EnrollmentService> _logger;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepository,
            ICourseRepository courseRepository,
            IStudentService studentService,
            ILogger<EnrollmentService> logger)
        {
            _enrollmentRepository = enrollmentRepository;
            _courseRepository = courseRepository;
            _studentService = studentService;
            _logger = logger;
        }

        public async Task<Enrollment> EnrollStudentAsync(string studentId, int courseId)
        {
            // Business rule validation: Check if student can enroll
            var canEnroll = await _studentService.CanStudentEnrollAsync(studentId, courseId);
            if (!canEnroll)
                throw new BusinessRuleViolationException("Student cannot enroll in this course");

            // Business rule: Check course availability
            var course = await _courseRepository.GetCourseByIdAsync(courseId);
            if (course == null)
                throw new EntityNotFoundException("Course not found");

            var currentEnrollmentCount = await _enrollmentRepository.GetCourseEnrollmentCountAsync(courseId);
            if (currentEnrollmentCount >= course.MaxStudents)
                throw new BusinessRuleViolationException("Course is full");

            var enrollment = new Enrollment
            {
                StudentId = studentId,
                CourseId = courseId,
                EnrollmentDate = DateTime.Now,
                Status = EnrollmentStatus.Active
            };

            var createdEnrollment = await _enrollmentRepository.CreateEnrollmentAsync(enrollment);
            _logger.LogInformation("Student {StudentId} enrolled in course {CourseId}", studentId, courseId);

            return createdEnrollment;
        }

        public async Task<Enrollment> UpdateGradeAsync(int enrollmentId, decimal grade)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new EntityNotFoundException("Enrollment not found");

            // Business rule: Grade must be between 0 and 100
            if (grade < 0 || grade > 100)
                throw new BusinessRuleViolationException("Grade must be between 0 and 100");

            // Business rule: Can only grade active enrollments
            if (enrollment.Status != EnrollmentStatus.Active)
                throw new BusinessRuleViolationException("Can only grade active enrollments");

            //enrollment.Grade = grade;

            //// Business rule: Automatically complete course if grade >= 60
            //if (grade >= 60)
            //{
            //    enrollment.Status = EnrollmentStatus.Completed;
            //}

            return await _enrollmentRepository.UpdateAsync(enrollment);
        }

        public async Task<bool> DropEnrollmentAsync(int enrollmentId)
        {
            var enrollment = await _enrollmentRepository.GetEnrollmentByIdAsync(enrollmentId);
            if (enrollment == null)
                throw new EntityNotFoundException("Enrollment not found");

            // Business rule: Cannot drop completed courses
            if (enrollment.Status == EnrollmentStatus.Completed)
                throw new BusinessRuleViolationException("Cannot drop completed courses");

            enrollment.Status = EnrollmentStatus.Dropped;
            await _enrollmentRepository.UpdateAsync(enrollment);

            return true;
        }

        public Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentIdAsync(string studentId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Enrollment>> GetEnrollmentsByCourseIdAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment?> GetEnrollmentAsync(string studentId, int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment> UpdateEnrollmentAsync(Enrollment enrollment)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteEnrollmentAsync(int enrollmentId)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetCourseEnrollmentCountAsync(int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> CanStudentEnrollAsync(string studentId, int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsStudentEnrolledInCourseAsync(string studentId, int courseId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsEnrollmentActiveAsync(int enrollmentId)
        {
            throw new NotImplementedException();
        }

        public Task<Enrollment?> GetEnrollmentByIdAsync(int enrollmentId)
        {
            throw new NotImplementedException();
        }


    }
}
