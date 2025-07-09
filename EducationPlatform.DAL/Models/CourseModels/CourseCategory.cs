using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EducationPlatform.BLL.IServices;



namespace EducationPlatform.DAL.Models.CourseModels
{
    public class CourseCategory : IAuditableEntity
    {

        [Key]
        public int CourseCategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        // Display Order
        public int DisplayOrder { get; set; }

        // Icon/Image
        [StringLength(500)]
        public string IconUrl { get; set; }

        [StringLength(500)]
        public string ImageUrl { get; set; }

        [StringLength(7)] // #RRGGBB format
        public string ColorCode { get; set; }

        // Status
        public bool IsActive { get; set; }

        // Navigation Property
        public virtual ICollection<Course> Courses { get; set; }

        // SEO Properties
        [StringLength(100)]
        public string Slug { get; set; }

        [StringLength(200)]
        public string MetaTitle { get; set; }

        [StringLength(500)]
        public string MetaDescription { get; set; }

        [StringLength(200)]
        public string Keywords { get; set; }

        // Audit Fields
        [Required]
        public DateTime CreatedUtc { get; set; }

        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        public DateTime? ModifiedUtc { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; }

        [Required]
        public DateTime LastModifiedAt { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Statistics (Not mapped to database)
        [NotMapped]
        public int ActiveCoursesCount => Courses?.Count(c => c.IsPublished && !c.IsDeleted) ?? 0;


    }
}