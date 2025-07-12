using EducationPlatform.BLL.DTOs;
using EducationPlatform.BLL.IServices;
using EducationPlatform.BLL.Services;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using EducationPlatform.DAL.Repositories;
using EducationPlatform.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EducationPlatform.WEB.Controllers
{

    public class CoursesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CoursesController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ICourseService _courseService;
        private readonly ICourseCategoryService _categoryService;

        public CoursesController(
            IUnitOfWork unitOfWork,
            ILogger<CoursesController> logger,
            IWebHostEnvironment webHostEnvironment,
            ICourseService courseService,
            ICourseCategoryService categoryService
            )
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
            _courseService = courseService ?? throw new ArgumentNullException(nameof(courseService));
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }

        // GET: Course
        public async Task<IActionResult> Index(string searchTerm, BLL.DTOs.CourseStatus? filterStatus,
            BLL.DTOs.CourseDifficulty? filterDifficulty, int? filterCategoryId, string sortBy = "CreatedAt",
            string sortDirection = "desc", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var courses = await _courseService.GetAllCoursesAsync();
                var courseViewModels = courses.Select(c => new CourseViewModel()
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    IsPublished = c.IsPublished,
                    
                    CreatedAt = c.CreatedAt,
                    
                    Price = c.Price,
                    IsFree = c.IsFree,
                    InstructorId = c.InstructorId
                }).AsQueryable(); 

                // Apply filters
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    courseViewModels = courseViewModels.Where(c =>
                        c.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                        c.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
                }

                if (filterStatus.HasValue)
                {
                    courseViewModels = courseViewModels.Where(c => c.Status == filterStatus.Value);
                }

                if (filterDifficulty.HasValue)
                {
                    courseViewModels = courseViewModels.Where(c => c.Difficulty == filterDifficulty.Value);
                }

                if (filterCategoryId.HasValue)
                {
                    courseViewModels = courseViewModels.Where(c => c.CategoryId == filterCategoryId.Value);
                }

                // Apply sorting
                courseViewModels = sortBy.ToLower() switch
                {
                    "title" => sortDirection == "asc" ? courseViewModels.OrderBy(c => c.Title) : courseViewModels.OrderByDescending(c => c.Title),
                    "createdat" => sortDirection == "asc" ? courseViewModels.OrderBy(c => c.CreatedAt) : courseViewModels.OrderByDescending(c => c.CreatedAt),
                    "status" => sortDirection == "asc" ? courseViewModels.OrderBy(c => c.Status) : courseViewModels.OrderByDescending(c => c.Status),
                    "difficulty" => sortDirection == "asc" ? courseViewModels.OrderBy(c => c.Difficulty) : courseViewModels.OrderByDescending(c => c.Difficulty),
                    _ => courseViewModels.OrderByDescending(c => c.CreatedAt)
                };

                var totalCount = courseViewModels.Count();
                var paginatedCourses = courseViewModels
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var viewModel = new CourseListViewModel
                {
                    Courses = paginatedCourses,
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    SearchTerm = searchTerm,
                    FilterStatus = filterStatus,
                    FilterDifficulty = filterDifficulty,
                    FilterCategoryId = filterCategoryId,
                    SortBy = sortBy,
                    SortDirection = sortDirection
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving courses");
                TempData["ErrorMessage"] = "An error occurred while loading courses.";
                return View(new CourseListViewModel());
            }
        }

        // GET: Course/Details/5
        //public async Task<IActionResult> Details(int id)
        //{
        //    try
        //    {
        //        var course = await _courseService.GetCourseWithDetailsAsync(id);
        //        if (course == null)
        //        {
        //            TempData["ErrorMessage"] = "Course not found.";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        var viewModel = new CourseViewModel(course);
        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving course with ID {CourseId}", id);
        //        TempData["ErrorMessage"] = "An error occurred while loading the course details.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        // GET: Course/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new CourseFormViewModel
            {
                // Set default values
                IsFree = true,
                Status = BLL.DTOs.CourseStatus.Draft,
                Difficulty = BLL.DTOs.CourseDifficulty.Beginner,
                CreatedAt = DateTime.Now,
                LastModifiedAt = DateTime.Now
            };

            await PopulateDropdownLists(viewModel);
            return View(viewModel);
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await PopulateDropdownLists(viewModel);
                    return View(viewModel);
                }

                // Validate price if course is not free
                if (!viewModel.IsFree && (!viewModel.Price.HasValue || viewModel.Price <= 0))
                {
                    ModelState.AddModelError(nameof(viewModel.Price), "Price is required for paid courses");
                    await PopulateDropdownLists(viewModel);
                    return View(viewModel);
                }

                // Handle file uploads
                if (viewModel.ThumbnailFile != null)
                {
                    var thumbnailResult = await ValidateAndSaveFile(viewModel.ThumbnailFile, "thumbnails", new[] { ".jpg", ".jpeg", ".png" }, 2);
                    if (!thumbnailResult.Success)
                    {
                        ModelState.AddModelError(nameof(viewModel.ThumbnailFile), thumbnailResult.ErrorMessage);
                        await PopulateDropdownLists(viewModel);
                        return View(viewModel);
                    }
                    viewModel.ThumbnailUrl = thumbnailResult.FilePath;
                }

                if (viewModel.PreviewVideoFile != null)
                {
                    var videoResult = await ValidateAndSaveFile(viewModel.PreviewVideoFile, "videos", new[] { ".mp4", ".mov", ".avi" }, 10);
                    if (!videoResult.Success)
                    {
                        ModelState.AddModelError(nameof(viewModel.PreviewVideoFile), videoResult.ErrorMessage);
                        await PopulateDropdownLists(viewModel);
                        return View(viewModel);
                    }
                    viewModel.PreviewVideoUrl = videoResult.FilePath;
                }

                // Set audit fields
                viewModel.InstructorId = GetCurrentUserId();
                viewModel.CreatedBy = User.Identity?.Name ?? "System";
                viewModel.CreatedAt = DateTime.Now;
                viewModel.LastModifiedAt = DateTime.Now;
                viewModel.CreatedUtc = DateTime.UtcNow;

                // Map ViewModel to DTO
                var courseDto = new CourseDto
                {
                    Title = viewModel.Title,
                    Description = viewModel.Description,
                    IsPublished = viewModel.IsPublished,
                    Status = viewModel.Status,
                    Difficulty = viewModel.Difficulty,
                    Price = viewModel.IsFree ? null : viewModel.Price,
                    IsFree = viewModel.IsFree,
                    InstructorId = viewModel.InstructorId,
                    CreatedAt = viewModel.CreatedAt,
                    ThumbnailUrl = viewModel.ThumbnailUrl,
                    PreviewVideoUrl = viewModel.PreviewVideoUrl,
                    Requirements = viewModel.Requirements,
                    LearningObjectives = viewModel.LearningObjectives,
                    CategoryId = viewModel.CategoryId,
                    EstimatedDuration = viewModel.EstimatedDuration,
                    
                };

                var createdCourse = await _courseService.CreateCourseAsync(courseDto);
                await _courseService.SaveChangesAsync();

                TempData["SuccessMessage"] = $"Course '{createdCourse.Title}' created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating course: {Message}", ex.Message);
                TempData["ErrorMessage"] = "An unexpected error occurred while creating the course. Please try again.";

                await PopulateDropdownLists(viewModel);
                return View(viewModel);
            }
        }

        private async Task<(bool Success, string FilePath, string ErrorMessage)> ValidateAndSaveFile(
            IFormFile file,
            string subDirectory,
            string[] allowedExtensions,
            int maxSizeMB)
        {
            // Validate file size
            if (file.Length > maxSizeMB * 1024 * 1024)
            {
                return (false, null, $"File size exceeds {maxSizeMB}MB limit");
            }

            // Validate file extension
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return (false, null, $"Only {string.Join(", ", allowedExtensions)} files are allowed");
            }

            try
            {
                var filePath = await SaveFileAsync(file, subDirectory);
                return (true, filePath, null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", file.FileName);
                return (false, null, "Error saving file. Please try again.");
            }
        }

        private async Task PopulateDropdownLists(CourseDto? course = null)
        {
            try
            {
                // Populate Categories dropdown
                var categories = await _categoryService.GetCategoriesForDropdownAsync();
                ViewBag.CategoryId = new SelectList(categories, "CourseCategoryId", "Name", course?.CategoryId);

                // Populate Course Status dropdown
                var statusList = Enum.GetValues(typeof(BLL.DTOs.CourseStatus))
                    .Cast<BLL.DTOs.CourseStatus>()
                    .Select(e => new SelectListItem
                    {
                        Value = ((int)e).ToString(),
                        Text = e.ToString(),
                        Selected = course?.Status == e
                    }).ToList();
                ViewBag.Status = statusList;

                // Populate Course Difficulty dropdown
                var difficultyList = Enum.GetValues(typeof(BLL.DTOs.CourseDifficulty))
                    .Cast<BLL.DTOs.CourseDifficulty>()
                    .Select(e => new SelectListItem
                    {
                        Value = ((int)e).ToString(),
                        Text = e.ToString(),
                        Selected = course?.Difficulty == e
                    }).ToList();
                ViewBag.Difficulty = difficultyList;

                // Populate Instructors dropdown 
                // var instructors = await _instructorService.GetActiveInstructorsAsync();
                // ViewBag.InstructorId = new SelectList(instructors, "Id", "FullName", course?.InstructorId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while populating dropdown lists for course form.");
                // Set empty lists to prevent view errors
                ViewBag.CategoryId = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
                ViewBag.Status = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
                ViewBag.Difficulty = new SelectList(Enumerable.Empty<SelectListItem>(), "Value", "Text");
            }
        }

        //// GET: Course/Edit/5
        //public async Task<IActionResult> Edit(int id)
        //{
        //    try
        //    {
        //        var course = await _courseService.GetCourseWithDetailsAsync(id);
        //        if (course == null)
        //        {
        //            TempData["ErrorMessage"] = "Course not found.";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        // Check if current user is the instructor or has admin rights
        //        var currentUserId = GetCurrentUserId();
        //        if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
        //        {
        //            TempData["ErrorMessage"] = "You don't have permission to edit this course.";
        //            return RedirectToAction(nameof(Details), new { id });
        //        }

        //        var viewModel = new CourseFormViewModel(course);
        //        await PopulateDropdownLists(viewModel);
        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving course for editing with ID {CourseId}", id);
        //        TempData["ErrorMessage"] = "An error occurred while loading the course for editing.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        //// POST: Course/Edit/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, CourseFormViewModel viewModel)
        //{
        //    if (id != viewModel.CourseId)
        //    {
        //        TempData["ErrorMessage"] = "Invalid course ID.";
        //        return RedirectToAction(nameof(Index));
        //    }

        //    try
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            var existingCourse = await _courseService.Courses.GetCourseWithDetailsAsync(id);
        //            if (existingCourse == null)
        //            {
        //                TempData["ErrorMessage"] = "Course not found.";
        //                return RedirectToAction(nameof(Index));
        //            }

        //            // Check permissions
        //            var currentUserId = GetCurrentUserId();
        //            if (existingCourse.InstructorId != currentUserId && !User.IsInRole("Admin"))
        //            {
        //                TempData["ErrorMessage"] = "You don't have permission to edit this course.";
        //                return RedirectToAction(nameof(Details), new { id });
        //            }

        //            // Handle file uploads
        //            if (viewModel.ThumbnailFile != null)
        //            {
        //                viewModel.ThumbnailUrl = await SaveFileAsync(viewModel.ThumbnailFile, "thumbnails");
        //            }
        //            else
        //            {
        //                viewModel.ThumbnailUrl = existingCourse.ThumbnailUrl;
        //            }

        //            if (viewModel.PreviewVideoFile != null)
        //            {
        //                viewModel.PreviewVideoUrl = await SaveFileAsync(viewModel.PreviewVideoFile, "videos");
        //            }
        //            else
        //            {
        //                viewModel.PreviewVideoUrl = existingCourse.PreviewVideoUrl;
        //            }

        //            // Update audit fields
        //            viewModel.ModifiedUtc = DateTime.UtcNow;
        //            viewModel.ModifiedBy = User.Identity?.Name ?? "MohamadEnma";
        //            viewModel.LastModifiedAt = DateTime.Now;

        //            // Preserve original creation data
        //            viewModel.CreatedAt = existingCourse.CreatedAt;
        //            viewModel.CreatedUtc = existingCourse.CreatedUtc;
        //            viewModel.CreatedBy = existingCourse.CreatedBy;
        //            viewModel.InstructorId = existingCourse.InstructorId;

        //            var updatedCourse = viewModel.ToEntity();
        //            _courseService.Courses.UpdateCourse(updatedCourse);
        //            await _courseService.SaveChangesAsync();

        //            TempData["SuccessMessage"] = "Course updated successfully!";
        //            return RedirectToAction(nameof(Details), new { id });
        //        }

        //        await PopulateDropdownLists(viewModel);
        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while updating course with ID {CourseId}", id);
        //        TempData["ErrorMessage"] = "An error occurred while updating the course.";
        //        await PopulateDropdownLists(viewModel);
        //        return View(viewModel);
        //    }
        //}

        // GET: Course/Delete/5
        //public async Task<IActionResult> Delete(int id)
        //{
        //    try
        //    {
        //        var course = await _courseService.Courses.GetCourseWithDetailsAsync(id);
        //        if (course == null)
        //        {
        //            TempData["ErrorMessage"] = "Course not found.";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        // Check permissions
        //        var currentUserId = GetCurrentUserId();
        //        if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
        //        {
        //            TempData["ErrorMessage"] = "You don't have permission to delete this course.";
        //            return RedirectToAction(nameof(Details), new { id });
        //        }

        //        var viewModel = new CourseViewModel(course);
        //        return View(viewModel);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving course for deletion with ID {CourseId}", id);
        //        TempData["ErrorMessage"] = "An error occurred while loading the course for deletion.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        //// POST: Course/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    try
        //    {
        //        var course = await _courseService.Courses.GetCourseWithDetailsAsync(id);
        //        if (course == null)
        //        {
        //            TempData["ErrorMessage"] = "Course not found.";
        //            return RedirectToAction(nameof(Index));
        //        }

        //        // Check permissions
        //        var currentUserId = GetCurrentUserId();
        //        if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
        //        {
        //            TempData["ErrorMessage"] = "You don't have permission to delete this course.";
        //            return RedirectToAction(nameof(Details), new { id });
        //        }

        //        // Soft delete
        //        course.IsDeleted = true;
        //        course.DeletedAt = DateTime.UtcNow;
        //        course.DeletedBy = User.Identity?.Name ?? "MohamadEnma";

        //        _courseService.Courses.UpdateCourse(course);
        //        await _courseService.SaveChangesAsync();

        //        TempData["SuccessMessage"] = "Course deleted successfully!";
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while deleting course with ID {CourseId}", id);
        //        TempData["ErrorMessage"] = "An error occurred while deleting the course.";
        //        return RedirectToAction(nameof(Index));
        //    }
        //}

        //// GET: Course/MyCourses
        //public async Task<IActionResult> MyCourses()
        //{
        //    try
        //    {
        //        var currentUserId = GetCurrentUserId();
        //        var courses = await _courseService.Courses.GetCoursesByInstructorAsync(currentUserId);
        //        var courseViewModels = courses.Select(c => new CourseViewModel(c));

        //        return View(courseViewModels);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving instructor courses");
        //        TempData["ErrorMessage"] = "An error occurred while loading your courses.";
        //        return View(new List<CourseViewModel>());
        //    }
        //}

        //// GET: Course/Published
        //[AllowAnonymous]
        //public async Task<IActionResult> Published()
        //{
        //    try
        //    {
        //        var courses = await _courseService.Courses.GetPublishedCoursesAsync();
        //        var courseViewModels = courses.Select(c => new CourseViewModel(c));

        //        return View(courseViewModels);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while retrieving published courses");
        //        TempData["ErrorMessage"] = "An error occurred while loading published courses.";
        //        return View(new List<CourseViewModel>());
        //    }
        //}

        //// POST: Course/Publish/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Publish(int id)
        //{
        //    try
        //    {
        //        var course = await _courseService.Courses.GetCourseWithDetailsAsync(id);
        //        if (course == null)
        //        {
        //            return Json(new { success = false, message = "Course not found." });
        //        }

        //        // Check permissions
        //        var currentUserId = GetCurrentUserId();
        //        if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
        //        {
        //            return Json(new { success = false, message = "You don't have permission to publish this course." });
        //        }

        //        course.IsPublished = true;
        //        course.PublishedAt = DateTime.UtcNow;
        //        course.Status = CourseStatus.Active;
        //        course.ModifiedUtc = DateTime.UtcNow;
        //        course.ModifiedBy = User.Identity?.Name ?? "MohamadEnma";

        //        _courseService.Courses.UpdateCourse(course);
        //        await _courseService.SaveChangesAsync();

        //        return Json(new { success = true, message = "Course published successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while publishing course with ID {CourseId}", id);
        //        return Json(new { success = false, message = "An error occurred while publishing the course." });
        //    }
        //}

        //// POST: Course/Unpublish/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Unpublish(int id)
        //{
        //    try
        //    {
        //        var course = await _courseService.Courses.GetCourseWithDetailsAsync(id);
        //        if (course == null)
        //        {
        //            return Json(new { success = false, message = "Course not found." });
        //        }

        //        // Check permissions
        //        var currentUserId = GetCurrentUserId();
        //        if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
        //        {
        //            return Json(new { success = false, message = "You don't have permission to unpublish this course." });
        //        }

        //        course.IsPublished = false;
        //        course.Status = CourseStatus.Draft;
        //        course.ModifiedUtc = DateTime.UtcNow;
        //        course.ModifiedBy = User.Identity?.Name ?? "MohamadEnma";

        //        _courseService.Courses.UpdateCourse(course);
        //        await _courseService.SaveChangesAsync();

        //        return Json(new { success = true, message = "Course unpublished successfully!" });
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error occurred while unpublishing course with ID {CourseId}", id);
        //        return Json(new { success = false, message = "An error occurred while unpublishing the course." });
        //    }
        //}

        //// Helper Methods
        private async Task PopulateDropdownLists(CourseViewModel viewModel)
        {
            // Status dropdown
            viewModel.StatusList = Enum.GetValues<BLL.DTOs.CourseStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString(),
                    Selected = s == viewModel.Status
                });

            // Difficulty dropdown
            viewModel.DifficultyList = Enum.GetValues<BLL.DTOs.CourseDifficulty>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString(),
                    Selected = d == viewModel.Difficulty
                });

            // Instructor dropdown (for admin users)
            //if (User.IsInRole("Admin"))
            //{
            //    var students = await _courseService.GetAllStudentsAsync();
            //    viewModel.InstructorList = students.Select(s => new SelectListItem
            //    {
            //        Value = s.Id,
            //        Text = s.UserName ?? s.Email,
            //        Selected = s.Id == viewModel.InstructorId
            //    });
            //}
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{folder}/{uniqueFileName}";
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "MohamadEnma";
        }
    }
}