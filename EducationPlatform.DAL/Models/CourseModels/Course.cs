using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.UserModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Models.CourseModels
{
    public class Course
    {
        [Key]
        public int CourseId { get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "Course Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Course Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        // course setting
        [Required]
        public bool IsPublished { get; set; } = false;
        [Required]
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        [Required]
        public CourseDifficulty Difficulty { get; set; }

        // Dates
        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime LastModifiedAt { get; set; }

        public DateTime? PublishedAt { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        // Duration in minutes
        public int? EstimatedDuration { get; set; }

        // Pricing
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public bool IsFree { get; set; }

        // Instructor/Owner
        [Required]
        public string InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual ApplicationUser Instructor { get; set; }

        // Course Category/Topic
        public int? CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual CourseCategory Category { get; set; }

        // Thumbnail/Preview
        [StringLength(500)]
        public string ThumbnailUrl { get; set; }

        [StringLength(500)]
        public string PreviewVideoUrl { get; set; }

        // Requirements and Learning Objectives
        [StringLength(4000)]
        public string Requirements { get; set; }

        [StringLength(4000)]
        public string LearningObjectives { get; set; }

        // Audit Fields
        [Required]
        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; } = "MohamadEnma"; // Current user's login

        public DateTime? ModifiedUtc { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime DeletedAt { get; set; }
        public string DeletedBy { get; set; }
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
