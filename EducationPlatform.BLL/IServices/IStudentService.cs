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
        Task<StudentDto?> GetStudentByIdAsync(int id);
        Task<StudentDto> CreateStudentAsync(StudentDto studentDto);
        Task<StudentDto?> UpdateStudentAsync(int id, StudentDto studentDto);
        Task<bool> DeleteStudentAsync(int id);
    }
}
