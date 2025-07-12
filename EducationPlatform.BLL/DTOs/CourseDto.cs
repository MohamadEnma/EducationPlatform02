using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.UserModels;
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

        // Dates 
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime ModifiedUtc { get; set; }
        public DateTime CreatedUtc { get; set; }


        public int MaxStudents { get; set; }
        public int EstimatedDuration { get; set; } // in hours 
        public decimal? Price { get; set; }
        public bool IsFree { get; set; }


        public ApplicationUser Instructor { get; set; } = new ApplicationUser();
        public string InstructorId { get; set; } = string.Empty;

        public CourseCategory Category { get; set; } = new CourseCategory();
        public int CategoryId { get; set; }


        // Thumbnail/Preview

        public string ThumbnailUrl { get; set; } = string.Empty;
        public string PreviewVideoUrl { get; set; } = string.Empty;

        // Requirements and Learning Objectives

        public string Requirements { get; set; } = string.Empty;
        public string LearningObjectives { get; set; } = string.Empty;

        // Audit Fields

        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;


        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; } = string.Empty;



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
