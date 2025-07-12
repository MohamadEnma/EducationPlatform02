using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.DTOs
{
    public class CourseCategoryDto
    {
        public int CourseCategoryId { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [StringLength(100, ErrorMessage = "Category name must be between 1 and 100 characters")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(0, int.MaxValue, ErrorMessage = "Display order must be a positive number")]
        public int DisplayOrder { get; set; }

        [StringLength(500, ErrorMessage = "Icon URL must not exceed 500 characters")]
        public string IconUrl { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Image URL must not exceed 500 characters")]
        public string ImageUrl { get; set; } = string.Empty;

        [StringLength(7, ErrorMessage = "Color code must be in #RRGGBB format")]
        [RegularExpression(@"^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color code must be in #RRGGBB format")]
        public string ColorCode { get; set; } = string.Empty;

        public bool IsActive { get; set; }

        [Required(ErrorMessage = "Slug is required")]
        [StringLength(100, ErrorMessage = "Slug must not exceed 100 characters")]
        public string Slug { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Meta title must not exceed 200 characters")]
        public string MetaTitle { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Meta description must not exceed 500 characters")]
        public string MetaDescription { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Keywords must not exceed 200 characters")]
        public string Keywords { get; set; } = string.Empty;

        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? ModifiedUtc { get; set; }
        public string ModifiedBy { get; set; } = string.Empty;

        // Additional properties for display
        public int ActiveCoursesCount { get; set; }
        public IEnumerable<CourseDto> Courses { get; set; } = new List<CourseDto>();
    }
}
