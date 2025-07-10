using EducationPlatform.DAL.Models.CourseModels;
using EducationPlatform.DAL.Models.UserModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EducationPlatform.WEB.ViewModels
{
    public class CourseViewModel
    {

        public int CourseId { get; set; }

       

        [Required(ErrorMessage = "Course Title is required.")]
        [StringLength(100, ErrorMessage = "Course Title cannot exceed 100 characters.")]
        [Display(Name = "Course Title")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Course Description is required.")]
        [StringLength(500, ErrorMessage = "Course Description cannot exceed 500 characters.")]
        [Display(Name = "Description")]
        public string Description { get; set; }

        // Course Settings
        [Display(Name = "Is Published")]
        public bool IsPublished { get; set; } = false;

        [Required(ErrorMessage = "Course Status is required.")]
        [Display(Name = "Status")]
        public CourseStatus Status { get; set; } = CourseStatus.Draft;

        [Required(ErrorMessage = "Course Difficulty is required.")]
        [Display(Name = "Difficulty Level")]
        public CourseDifficulty Difficulty { get; set; }

        // Dates
        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }

        [Display(Name = "Last Modified")]
        public DateTime LastModifiedAt { get; set; }

        [Display(Name = "Published At")]
        public DateTime? PublishedAt { get; set; }

        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime? StartDate { get; set; }

        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        // Duration
        [Display(Name = "Estimated Duration (minutes)")]
        [Range(1, int.MaxValue, ErrorMessage = "Duration must be a positive number.")]
        public int? EstimatedDuration { get; set; }

        // Pricing
        [Display(Name = "Price")]
        [DataType(DataType.Currency)]
        [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive number.")]
        public decimal? Price { get; set; }

        [Display(Name = "Is Free")]
        public bool IsFree { get; set; }

        // Instructor
        [Required(ErrorMessage = "Instructor is required.")]
        [Display(Name = "Instructor")]
        public string InstructorId { get; set; }

        [Display(Name = "Instructor Name")]
        public string InstructorName { get; set; }

        // Category
        [Display(Name = "Category")]
        public int? CategoryId { get; set; }

        [Display(Name = "Category Name")]
        public string CategoryName { get; set; }

        // Media
        [Display(Name = "Thumbnail URL")]
        [StringLength(500, ErrorMessage = "Thumbnail URL cannot exceed 500 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string ThumbnailUrl { get; set; }

        [Display(Name = "Preview Video URL")]
        [StringLength(500, ErrorMessage = "Preview Video URL cannot exceed 500 characters.")]
        [Url(ErrorMessage = "Please enter a valid URL.")]
        public string PreviewVideoUrl { get; set; }

        // Course Details
        [Display(Name = "Requirements")]
        [StringLength(4000, ErrorMessage = "Requirements cannot exceed 4000 characters.")]
        public string Requirements { get; set; }

        [Display(Name = "Learning Objectives")]
        [StringLength(4000, ErrorMessage = "Learning Objectives cannot exceed 4000 characters.")]
        public string LearningObjectives { get; set; }

        // Audit Fields (for display only)
        [Display(Name = "Created")]
        public DateTime CreatedUtc { get; set; }

        [Display(Name = "Created By")]
        public string CreatedBy { get; set; }

        [Display(Name = "Modified")]
        public DateTime? ModifiedUtc { get; set; }

        [Display(Name = "Modified By")]
        public string ModifiedBy { get; set; }

        // Additional Properties for Views
        [Display(Name = "Duration (Hours)")]
        public string FormattedDuration
        {
            get
            {
                if (EstimatedDuration.HasValue)
                {
                    var hours = EstimatedDuration.Value / 60;
                    var minutes = EstimatedDuration.Value % 60;
                    return hours > 0 ? $"{hours}h {minutes}m" : $"{minutes}m";
                }
                return "Not specified";
            }
        }

        [Display(Name = "Price Display")]
        public string FormattedPrice
        {
            get
            {
                if (IsFree)
                    return "Free";
                return Price.HasValue ? $"${Price.Value:F2}" : "Not specified";
            }
        }

        [Display(Name = "Status Badge")]
        public string StatusBadgeClass
        {
            get
            {
                return Status switch
                {
                    CourseStatus.Draft => "badge-secondary",
                    CourseStatus.PendingReview => "badge-warning",
                    CourseStatus.Active => "badge-success",
                    CourseStatus.Inactive => "badge-danger",
                    CourseStatus.Archived => "badge-dark",
                    _ => "badge-light"
                };
            }
        }

        [Display(Name = "Difficulty Badge")]
        public string DifficultyBadgeClass
        {
            get
            {
                return Difficulty switch
                {
                    CourseDifficulty.Beginner => "badge-success",
                    CourseDifficulty.Intermediate => "badge-warning",
                    CourseDifficulty.Advanced => "badge-danger",
                    CourseDifficulty.Expert => "badge-dark",
                    _ => "badge-light"
                };
            }
        }

        // Navigation Properties for Dropdowns
        public IEnumerable<SelectListItem> StatusList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> DifficultyList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> CategoryList { get; set; } = new List<SelectListItem>();
        public IEnumerable<SelectListItem> InstructorList { get; set; } = new List<SelectListItem>();

        // Constructor
        public CourseViewModel()
        {
            CreatedAt = DateTime.Now;
            LastModifiedAt = DateTime.Now;
            CreatedUtc = DateTime.UtcNow;
            CreatedBy = "MohamadEnma";
        }

        // Constructor for mapping from Course entity
        public CourseViewModel(Course course)
        {
            if (course != null)
            {
                CourseId = course.CourseId;
                Title = course.Title;
                Description = course.Description;
                IsPublished = course.IsPublished;
                Status = course.Status;
                Difficulty = course.Difficulty;
                CreatedAt = course.CreatedAt;
                LastModifiedAt = course.LastModifiedAt;
                PublishedAt = course.PublishedAt;
                StartDate = course.StartDate;
                EndDate = course.EndDate;
                EstimatedDuration = course.EstimatedDuration;
                Price = course.Price;
                IsFree = course.IsFree;
                InstructorId = course.InstructorId;
                InstructorName = course.Instructor?.UserName ?? course.Instructor?.Email;
                CategoryId = course.CategoryId;
                CategoryName = course.Category?.Name;
                ThumbnailUrl = course.ThumbnailUrl;
                PreviewVideoUrl = course.PreviewVideoUrl;
                Requirements = course.Requirements;
                LearningObjectives = course.LearningObjectives;
                CreatedUtc = course.CreatedUtc;
                CreatedBy = course.CreatedBy;
                ModifiedUtc = course.ModifiedUtc;
                ModifiedBy = course.ModifiedBy;
            }
        }

        // Method to convert ViewModel to Course entity
        public Course ToEntity()
        {
            return new Course
            {
                CourseId = CourseId,
                Title = Title,
                Description = Description,
                IsPublished = IsPublished,
                Status = Status,
                Difficulty = Difficulty,
                CreatedAt = CreatedAt,
                LastModifiedAt = DateTime.Now,
                PublishedAt = PublishedAt,
                StartDate = StartDate,
                EndDate = EndDate,
                EstimatedDuration = EstimatedDuration,
                Price = Price,
                IsFree = IsFree,
                InstructorId = InstructorId,
                CategoryId = CategoryId,
                ThumbnailUrl = ThumbnailUrl,
                PreviewVideoUrl = PreviewVideoUrl,
                Requirements = Requirements,
                LearningObjectives = LearningObjectives,
                CreatedUtc = CourseId == 0 ? DateTime.UtcNow : CreatedUtc,
                CreatedBy = CourseId == 0 ? "MohamadEnma" : CreatedBy,
                ModifiedUtc = DateTime.UtcNow,
                ModifiedBy = "MohamadEnma"
            };
        }
    }

    // Helper ViewModel for Course Lists
    public class CourseListViewModel
    {
        public IEnumerable<CourseViewModel> Courses { get; set; } = new List<CourseViewModel>();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string SearchTerm { get; set; }
        public CourseStatus? FilterStatus { get; set; }
        public CourseDifficulty? FilterDifficulty { get; set; }
        public int? FilterCategoryId { get; set; }
        public string SortBy { get; set; } = "CreatedAt";
        public string SortDirection { get; set; } = "desc";

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;
    }

    // ViewModel for Course Creation/Edit Forms
    public class CourseFormViewModel : CourseViewModel
    {
        public CourseFormViewModel()
        {
        }

        public CourseFormViewModel(Course course) : base(course)
        {
        }

        public bool IsEditMode => CourseId > 0;

        [Display(Name = "Upload Thumbnail")]
        public IFormFile ThumbnailFile { get; set; }

        [Display(Name = "Upload Preview Video")]
        public IFormFile PreviewVideoFile { get; set; }
    }
   
}