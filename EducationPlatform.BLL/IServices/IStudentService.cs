using EducationPlatform.BLL.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.IServices
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllStudentsAsync();
        Task<StudentDto?> GetStudentByIdAsync(string id);
        Task<StudentDto> CreateStudentAsync(StudentDto studentDto);
        Task<StudentDto?> UpdateStudentAsync(string id, StudentDto studentDto);
        Task<bool> DeleteStudentAsync(string id);
        Task<bool> CanStudentEnrollAsync(string studentId, int courseId);
    }
}
