using EducationPlatform.BLL.DTOs;
using EducationPlatform.BLL.IServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.Services
{
    public class StudentService : IStudentService
    {
        public Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
        {
            throw new NotImplementedException();
        }
        public Task<StudentDto?> GetStudentByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
        public Task<StudentDto> CreateStudentAsync(StudentDto studentDto)
        {
            throw new NotImplementedException();
        }
        public Task<StudentDto?> UpdateStudentAsync(int id, StudentDto studentDto)
        {
            throw new NotImplementedException();
        }
        public Task<bool> DeleteStudentAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
    
}
