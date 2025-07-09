using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Course title is required")]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course description is required")]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public bool IsPublished { get; set; }
        public CourseStatus Status { get; set; }
        public CourseDifficulty Difficulty { get; set; }
        public decimal? Price { get; set; }
        public bool IsFree { get; set; }
        public string InstructorId { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    public enum CourseStatus
    {
        Draft,
        PendingReview,
        Active,
        Inactive,
        Archived
    }

    public enum CourseDifficulty
    {
        Beginner,
        Intermediate,
        Advanced,
        Expert
    }
}
