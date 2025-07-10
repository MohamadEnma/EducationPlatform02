using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.IRepositories
{
    public interface IAuditableEntity
    {
        DateTime CreatedUtc { get; set; }
        string CreatedBy { get; set; }
        DateTime? ModifiedUtc { get; set; }
        string ModifiedBy { get; set; }
    }
}
