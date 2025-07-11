using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using EducationPlatform.WEB.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace EducationPlatform.WEB.Controllers
{
    [Authorize]
    public class CoursesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CoursesController> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CoursesController(IUnitOfWork unitOfWork, ILogger<CoursesController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Course
        public async Task<IActionResult> Index(string searchTerm, CourseStatus? filterStatus,
            CourseDifficulty? filterDifficulty, int? filterCategoryId, string sortBy = "CreatedAt",
            string sortDirection = "desc", int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                // Use database-level filtering instead of in-memory filtering
                var coursesQuery = await _unitOfWork.Courses.GetCoursesQueryableAsync();

                // Apply filters at database level
                if (!string.IsNullOrEmpty(searchTerm))
                {
                    coursesQuery = coursesQuery.Where(c =>
                        c.Title.Contains(searchTerm) ||
                        c.Description.Contains(searchTerm));
                }

                if (filterStatus.HasValue)
                {
                    coursesQuery = coursesQuery.Where(c => c.Status == filterStatus.Value);
                }

                if (filterDifficulty.HasValue)
                {
                    coursesQuery = coursesQuery.Where(c => c.Difficulty == filterDifficulty.Value);
                }

                if (filterCategoryId.HasValue)
                {
                    coursesQuery = coursesQuery.Where(c => c.CategoryId == filterCategoryId.Value);
                }

                // Apply sorting at database level
                coursesQuery = sortBy.ToLower() switch
                {
                    "title" => sortDirection == "asc" ? coursesQuery.OrderBy(c => c.Title) : coursesQuery.OrderByDescending(c => c.Title),
                    "createdat" => sortDirection == "asc" ? coursesQuery.OrderBy(c => c.CreatedAt) : coursesQuery.OrderByDescending(c => c.CreatedAt),
                    "status" => sortDirection == "asc" ? coursesQuery.OrderBy(c => c.Status) : coursesQuery.OrderByDescending(c => c.Status),
                    "difficulty" => sortDirection == "asc" ? coursesQuery.OrderBy(c => c.Difficulty) : coursesQuery.OrderByDescending(c => c.Difficulty),
                    _ => coursesQuery.OrderByDescending(c => c.CreatedAt)
                };

                // Get total count before pagination
                var totalCount = await _unitOfWork.Courses.GetCoursesCountAsync(coursesQuery);

                // Apply pagination at database level
                var courses = await _unitOfWork.Courses.GetPagedCoursesAsync(coursesQuery, pageNumber, pageSize);

                // Convert to view models
                var courseViewModels = courses.Select(c => new CourseViewModel(c)).ToList();

                var viewModel = new CourseListViewModel
                {
                    Courses = courseViewModels,
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
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = new CourseViewModel(course);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving course with ID {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the course details.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Course/Create
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Create()
        {
            var viewModel = new CourseFormViewModel();
            await PopulateDropdownLists(viewModel);
            return View(viewModel);
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Instructor")]
        public async Task<IActionResult> Create(CourseFormViewModel viewModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        // Handle file uploads
                        if (viewModel.ThumbnailFile != null)
                        {
                            viewModel.ThumbnailUrl = await SaveFileAsync(viewModel.ThumbnailFile, "thumbnails");
                        }

                        if (viewModel.PreviewVideoFile != null)
                        {
                            viewModel.PreviewVideoUrl = await SaveFileAsync(viewModel.PreviewVideoFile, "videos");
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        await PopulateDropdownLists(viewModel);
                        return View(viewModel);
                    }

                    // Set audit fields
                    viewModel.InstructorId = GetCurrentUserId();
                    viewModel.CreatedAt = DateTime.Now;
                    viewModel.LastModifiedAt = DateTime.Now;
                    viewModel.CreatedUtc = DateTime.UtcNow;
                    viewModel.CreatedBy = User.Identity?.Name ?? "MohamadEnma";

                    var course = viewModel.ToEntity();
                    await _unitOfWork.Courses.AddCourseAsync(course);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Course created successfully!";
                    return RedirectToAction(nameof(Details), new { id = course.CourseId });
                }

                await PopulateDropdownLists(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating course");
                TempData["ErrorMessage"] = "An error occurred while creating the course.";
                await PopulateDropdownLists(viewModel);
                return View(viewModel);
            }
        }

        // GET: Course/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check if current user is the instructor or has admin rights
                var currentUserId = GetCurrentUserId();
                if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You don't have permission to edit this course.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var viewModel = new CourseFormViewModel(course);
                await PopulateDropdownLists(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving course for editing with ID {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the course for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseFormViewModel viewModel)
        {
            if (id != viewModel.CourseId)
            {
                TempData["ErrorMessage"] = "Invalid course ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var existingCourse = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                    if (existingCourse == null)
                    {
                        TempData["ErrorMessage"] = "Course not found.";
                        return RedirectToAction(nameof(Index));
                    }

                    // Check permissions
                    var currentUserId = GetCurrentUserId();
                    if (existingCourse.InstructorId != currentUserId && !User.IsInRole("Admin"))
                    {
                        TempData["ErrorMessage"] = "You don't have permission to edit this course.";
                        return RedirectToAction(nameof(Details), new { id });
                    }

                    try
                    {
                        // Handle file uploads
                        if (viewModel.ThumbnailFile != null)
                        {
                            viewModel.ThumbnailUrl = await SaveFileAsync(viewModel.ThumbnailFile, "thumbnails");
                        }
                        else
                        {
                            viewModel.ThumbnailUrl = existingCourse.ThumbnailUrl;
                        }

                        if (viewModel.PreviewVideoFile != null)
                        {
                            viewModel.PreviewVideoUrl = await SaveFileAsync(viewModel.PreviewVideoFile, "videos");
                        }
                        else
                        {
                            viewModel.PreviewVideoUrl = existingCourse.PreviewVideoUrl;
                        }
                    }
                    catch (InvalidOperationException ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        await PopulateDropdownLists(viewModel);
                        return View(viewModel);
                    }

                    // Update audit fields
                    viewModel.ModifiedUtc = DateTime.UtcNow;
                    viewModel.ModifiedBy = User.Identity?.Name ?? "MohamadEnma";
                    viewModel.LastModifiedAt = DateTime.Now;

                    // Preserve original creation data
                    viewModel.CreatedAt = existingCourse.CreatedAt;
                    viewModel.CreatedUtc = existingCourse.CreatedUtc;
                    viewModel.CreatedBy = existingCourse.CreatedBy;
                    viewModel.InstructorId = existingCourse.InstructorId;

                    var updatedCourse = viewModel.ToEntity();
                    _unitOfWork.Courses.UpdateCourse(updatedCourse);
                    await _unitOfWork.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Course updated successfully!";
                    return RedirectToAction(nameof(Details), new { id });
                }

                await PopulateDropdownLists(viewModel);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating course with ID {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while updating the course.";
                await PopulateDropdownLists(viewModel);
                return View(viewModel);
            }
        }

        // GET: Course/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check permissions
                var currentUserId = GetCurrentUserId();
                if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete this course.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                var viewModel = new CourseViewModel(course);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving course for deletion with ID {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the course for deletion.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Course/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                if (course == null)
                {
                    TempData["ErrorMessage"] = "Course not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Check permissions
                var currentUserId = GetCurrentUserId();
                if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
                {
                    TempData["ErrorMessage"] = "You don't have permission to delete this course.";
                    return RedirectToAction(nameof(Details), new { id });
                }

                // Soft delete
                course.IsDeleted = true;
                course.DeletedAt = DateTime.UtcNow;
                course.DeletedBy = User.Identity?.Name ?? "MohamadEnma";

                _unitOfWork.Courses.UpdateCourse(course);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Course deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting course with ID {CourseId}", id);
                TempData["ErrorMessage"] = "An error occurred while deleting the course.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Course/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            try
            {
                var currentUserId = GetCurrentUserId();
                var courses = await _unitOfWork.Courses.GetCoursesByInstructorAsync(currentUserId);
                var courseViewModels = courses.Select(c => new CourseViewModel(c));

                return View(courseViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving instructor courses");
                TempData["ErrorMessage"] = "An error occurred while loading your courses.";
                return View(new List<CourseViewModel>());
            }
        }

        // GET: Course/Published
        [AllowAnonymous]
        public async Task<IActionResult> Published()
        {
            try
            {
                var courses = await _unitOfWork.Courses.GetPublishedCoursesAsync();
                var courseViewModels = courses.Select(c => new CourseViewModel(c));

                return View(courseViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving published courses");
                TempData["ErrorMessage"] = "An error occurred while loading published courses.";
                return View(new List<CourseViewModel>());
            }
        }

        // POST: Course/Publish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                if (course == null)
                {
                    return Json(new { success = false, message = "Course not found." });
                }

                // Check permissions
                var currentUserId = GetCurrentUserId();
                if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "You don't have permission to publish this course." });
                }

                course.IsPublished = true;
                course.PublishedAt = DateTime.UtcNow;
                course.Status = CourseStatus.Active;
                course.ModifiedUtc = DateTime.UtcNow;
                course.ModifiedBy = User.Identity?.Name ?? "MohamadEnma";

                _unitOfWork.Courses.UpdateCourse(course);
                await _unitOfWork.SaveChangesAsync();

                return Json(new { success = true, message = "Course published successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while publishing course with ID {CourseId}", id);
                return Json(new { success = false, message = "An error occurred while publishing the course." });
            }
        }

        // POST: Course/Unpublish/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(int id)
        {
            try
            {
                var course = await _unitOfWork.Courses.GetCourseWithDetailsAsync(id);
                if (course == null)
                {
                    return Json(new { success = false, message = "Course not found." });
                }

                // Check permissions
                var currentUserId = GetCurrentUserId();
                if (course.InstructorId != currentUserId && !User.IsInRole("Admin"))
                {
                    return Json(new { success = false, message = "You don't have permission to unpublish this course." });
                }

                course.IsPublished = false;
                course.Status = CourseStatus.Draft;
                course.ModifiedUtc = DateTime.UtcNow;
                course.ModifiedBy = User.Identity?.Name ?? "MohamadEnma";

                _unitOfWork.Courses.UpdateCourse(course);
                await _unitOfWork.SaveChangesAsync();

                return Json(new { success = true, message = "Course unpublished successfully!" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while unpublishing course with ID {CourseId}", id);
                return Json(new { success = false, message = "An error occurred while unpublishing the course." });
            }
        }

        // Helper Methods
        private async Task PopulateDropdownLists(CourseFormViewModel viewModel)
        {
            // Status dropdown
            viewModel.StatusList = Enum.GetValues<CourseStatus>()
                .Select(s => new SelectListItem
                {
                    Value = ((int)s).ToString(),
                    Text = s.ToString(),
                    Selected = s == viewModel.Status
                });

            // Difficulty dropdown
            viewModel.DifficultyList = Enum.GetValues<CourseDifficulty>()
                .Select(d => new SelectListItem
                {
                    Value = ((int)d).ToString(),
                    Text = d.ToString(),
                    Selected = d == viewModel.Difficulty
                });

            // Category dropdown
            var categories = await _unitOfWork.Categories.GetAllActiveAsync();
            viewModel.CategoryList = categories.Select(c => new SelectListItem
            {
                Value = c.CourseCategoryId.ToString(),
                Text = c.Name,
                Selected = c.CourseCategoryId == viewModel.CategoryId
            });

            // Instructor dropdown (for admin users)
            if (User.IsInRole("Admin"))
            {
                var students = await _unitOfWork.Students.GetAllAsync();
                viewModel.InstructorList = students.Select(s => new SelectListItem
                {
                    Value = s.Id,
                    Text = s.UserName ?? s.Email,
                    Selected = s.Id == viewModel.InstructorId
                });
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            // File validation
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedVideoExtensions = new[] { ".mp4", ".avi", ".mov", ".wmv", ".webm" };
            
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (folder == "thumbnails" && !allowedImageExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"Invalid file type for thumbnail. Allowed types: {string.Join(", ", allowedImageExtensions)}");
            }
            
            if (folder == "videos" && !allowedVideoExtensions.Contains(fileExtension))
            {
                throw new InvalidOperationException($"Invalid file type for video. Allowed types: {string.Join(", ", allowedVideoExtensions)}");
            }

            // File size validation (10MB for images, 100MB for videos)
            var maxSize = folder == "thumbnails" ? 10 * 1024 * 1024 : 100 * 1024 * 1024;
            if (file.Length > maxSize)
            {
                var maxSizeMB = maxSize / (1024 * 1024);
                throw new InvalidOperationException($"File size exceeds the maximum allowed size of {maxSizeMB}MB.");
            }

            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folder);
            Directory.CreateDirectory(uploadsFolder);

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
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