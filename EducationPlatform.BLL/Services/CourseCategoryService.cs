using EducationPlatform.BLL.DTOs;
using EducationPlatform.BLL.IServices;
using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EducationPlatform.BLL.Services
{
    public class CourseCategoryService : ICourseCategoryService
    {
        private readonly ICourseCaegoryRepository _categoryRepository;
        private readonly ILogger<CourseCategoryService> _logger;

        public CourseCategoryService(
            ICourseCaegoryRepository categoryRepository,
            ILogger<CourseCategoryService> logger)
        {
            _categoryRepository = categoryRepository ?? throw new ArgumentNullException(nameof(categoryRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IEnumerable<CourseCategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllAsync();
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching all categories.");
                throw new ApplicationException("An error occurred while retrieving categories.", ex);
            }
        }

        public async Task<CourseCategoryDto?> GetCategoryByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("GetCategoryByIdAsync called with invalid id: {Id}", id);
                    return null;
                }

                var category = await _categoryRepository.GetCategoryByIdAsync(id);
                return category != null ? MapToDto(category) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category with id: {Id}", id);
                throw new ApplicationException($"An error occurred while retrieving the category with ID {id}.", ex);
            }
        }

        public async Task<CourseCategoryDto> CreateCategoryAsync(CourseCategoryDto categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    throw new ArgumentNullException(nameof(categoryDto), "CategoryDto cannot be null.");

                // Validate slug uniqueness
                if (!await IsCategorySlugUniqueAsync(categoryDto.Slug))
                    throw new InvalidOperationException($"A category with slug '{categoryDto.Slug}' already exists.");

                // Generate slug if not provided
                if (string.IsNullOrWhiteSpace(categoryDto.Slug))
                    categoryDto.Slug = await GenerateSlugAsync(categoryDto.Name);

                var category = new CourseCategory
                {
                    Name = categoryDto.Name,
                    Description = categoryDto.Description,
                    DisplayOrder = categoryDto.DisplayOrder,
                    IconUrl = categoryDto.IconUrl,
                    ImageUrl = categoryDto.ImageUrl,
                    ColorCode = categoryDto.ColorCode,
                    IsActive = categoryDto.IsActive,
                    Slug = categoryDto.Slug,
                    MetaTitle = categoryDto.MetaTitle,
                    MetaDescription = categoryDto.MetaDescription,
                    Keywords = categoryDto.Keywords,
                    CreatedUtc = DateTime.UtcNow,
                    CreatedBy = "MohamadEnma", // Using current user
                    LastModifiedAt = DateTime.Now
                };

                var createdCategory = await _categoryRepository.CreateCategoryAsync(category);
                if (createdCategory == null)
                    throw new InvalidOperationException("Failed to create category.");

                _logger.LogInformation("Category created successfully with ID: {CategoryId}", createdCategory.CourseCategoryId);
                return MapToDto(createdCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating category: {CategoryName}", categoryDto?.Name);
                throw new ApplicationException("An error occurred while creating the category.", ex);
            }
        }

        public async Task<CourseCategoryDto?> UpdateCategoryAsync(int id, CourseCategoryDto categoryDto)
        {
            try
            {
                if (categoryDto == null)
                    throw new ArgumentNullException(nameof(categoryDto), "CategoryDto cannot be null.");

                var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
                if (existingCategory == null)
                {
                    _logger.LogWarning("Category with ID {Id} not found for update.", id);
                    return null;
                }

                // Validate slug uniqueness (excluding current category)
                if (!await IsCategorySlugUniqueAsync(categoryDto.Slug, id))
                    throw new InvalidOperationException($"A category with slug '{categoryDto.Slug}' already exists.");

                // Update properties
                existingCategory.Name = categoryDto.Name;
                existingCategory.Description = categoryDto.Description;
                existingCategory.DisplayOrder = categoryDto.DisplayOrder;
                existingCategory.IconUrl = categoryDto.IconUrl;
                existingCategory.ImageUrl = categoryDto.ImageUrl;
                existingCategory.ColorCode = categoryDto.ColorCode;
                existingCategory.IsActive = categoryDto.IsActive;
                existingCategory.Slug = categoryDto.Slug;
                existingCategory.MetaTitle = categoryDto.MetaTitle;
                existingCategory.MetaDescription = categoryDto.MetaDescription;
                existingCategory.Keywords = categoryDto.Keywords;
                existingCategory.ModifiedUtc = DateTime.UtcNow;
                existingCategory.ModifiedBy = "MohamadEnma"; // Using current user
                existingCategory.LastModifiedAt = DateTime.Now;

                var updatedCategory = await _categoryRepository.UpdateCategoryAsync(existingCategory);
                _logger.LogInformation("Category updated successfully with ID: {CategoryId}", id);
                return MapToDto(updatedCategory);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating category with ID: {Id}", id);
                throw new ApplicationException($"An error occurred while updating the category with ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteCategoryAsync(int id)
        {
            try
            {
                if (!await CanDeleteCategoryAsync(id))
                {
                    _logger.LogWarning("Cannot delete category with ID {Id} - category has active courses.", id);
                    return false;
                }

                var result = await _categoryRepository.DeleteCategoryAsync(id);
                if (result)
                    _logger.LogInformation("Category deleted successfully with ID: {CategoryId}", id);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting category with ID: {Id}", id);
                throw new ApplicationException($"An error occurred while deleting the category with ID {id}.", ex);
            }
        }

        public async Task<bool> SoftDeleteCategoryAsync(int id, string deletedBy)
        {
            try
            {
                var result = await _categoryRepository.SoftDeleteCategoryAsync(id, deletedBy ?? "MohamadEnma");
                if (result)
                    _logger.LogInformation("Category soft deleted successfully with ID: {CategoryId} by {DeletedBy}", id, deletedBy);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while soft deleting category with ID: {Id}", id);
                throw new ApplicationException($"An error occurred while soft deleting the category with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<CourseCategoryDto>> GetActiveCategoriesAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllActiveCategoriesAsync();
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching active categories.");
                throw new ApplicationException("An error occurred while retrieving active categories.", ex);
            }
        }

        public async Task<IEnumerable<CourseCategoryDto>> GetCategoriesByDisplayOrderAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesByDisplayOrderAsync();
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories by display order.");
                throw new ApplicationException("An error occurred while retrieving categories by display order.", ex);
            }
        }

        public async Task<CourseCategoryDto?> GetCategoryBySlugAsync(string slug)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(slug))
                    return null;

                var category = await _categoryRepository.GetCategoryBySlugAsync(slug);
                return category != null ? MapToDto(category) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category by slug: {Slug}", slug);
                throw new ApplicationException($"An error occurred while retrieving the category with slug '{slug}'.", ex);
            }
        }

        public async Task<CourseCategoryDto?> GetCategoryWithCoursesAsync(int categoryId)
        {
            try
            {
                var category = await _categoryRepository.GetCategoryWithCoursesAsync(categoryId);
                return category != null ? MapToDtoWithCourses(category) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching category with courses for ID: {CategoryId}", categoryId);
                throw new ApplicationException($"An error occurred while retrieving the category with courses for ID {categoryId}.", ex);
            }
        }

        public async Task<IEnumerable<CourseCategoryDto>> GetCategoriesWithCourseCountAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetCategoriesWithCourseCountAsync();
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories with course count.");
                throw new ApplicationException("An error occurred while retrieving categories with course count.", ex);
            }
        }

        public async Task<IEnumerable<CourseCategoryDto>> SearchCategoriesAsync(string searchTerm)
        {
            try
            {
                var categories = await _categoryRepository.SearchCategoriesAsync(searchTerm);
                return categories.Select(MapToDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while searching categories with term: {SearchTerm}", searchTerm);
                throw new ApplicationException($"An error occurred while searching categories with term '{searchTerm}'.", ex);
            }
        }

        public async Task<bool> IsCategorySlugUniqueAsync(string slug, int? excludeCategoryId = null)
        {
            try
            {
                return await _categoryRepository.IsCategorySlugUniqueAsync(slug, excludeCategoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking slug uniqueness: {Slug}", slug);
                return false;
            }
        }

        public async Task<int> GetCoursesCountByCategoryAsync(int categoryId)
        {
            try
            {
                return await _categoryRepository.GetCoursesCountByCategoryAsync(categoryId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting course count for category: {CategoryId}", categoryId);
                return 0;
            }
        }

        public async Task<bool> CanDeleteCategoryAsync(int categoryId)
        {
            try
            {
                var courseCount = await GetCoursesCountByCategoryAsync(categoryId);
                return courseCount == 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while checking if category can be deleted: {CategoryId}", categoryId);
                return false;
            }
        }

        public async Task<string> GenerateSlugAsync(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                return string.Empty;

            // Convert to lowercase and replace spaces with hyphens
            var slug = categoryName.ToLowerInvariant()
                                  .Replace(" ", "-")
                                  .Replace("&", "and");

            // Remove special characters except hyphens
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");

            // Remove multiple consecutive hyphens
            slug = Regex.Replace(slug, @"-+", "-");

            // Remove leading and trailing hyphens
            slug = slug.Trim('-');

            // Ensure uniqueness
            var originalSlug = slug;
            var counter = 1;
            while (!await IsCategorySlugUniqueAsync(slug))
            {
                slug = $"{originalSlug}-{counter}";
                counter++;
            }

            return slug;
        }

        public async Task<IEnumerable<CourseCategoryDto>> GetCategoriesForDropdownAsync()
        {
            try
            {
                var categories = await _categoryRepository.GetAllActiveCategoriesAsync();
                return categories.Select(c => new CourseCategoryDto
                {
                    CourseCategoryId = c.CourseCategoryId,
                    Name = c.Name,
                    DisplayOrder = c.DisplayOrder
                }).OrderBy(c => c.DisplayOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching categories for dropdown.");
                throw new ApplicationException("An error occurred while retrieving categories for dropdown.", ex);
            }
        }

        #region Private Helper Methods

        private static CourseCategoryDto MapToDto(CourseCategory category)
        {
            return new CourseCategoryDto
            {
                CourseCategoryId = category.CourseCategoryId,
                Name = category.Name,
                Description = category.Description,
                DisplayOrder = category.DisplayOrder,
                IconUrl = category.IconUrl,
                ImageUrl = category.ImageUrl,
                ColorCode = category.ColorCode,
                IsActive = category.IsActive,
                Slug = category.Slug,
                MetaTitle = category.MetaTitle,
                MetaDescription = category.MetaDescription,
                Keywords = category.Keywords,
                CreatedUtc = category.CreatedUtc,
                CreatedBy = category.CreatedBy,
                ModifiedUtc = category.ModifiedUtc,
                ModifiedBy = category.ModifiedBy,
                ActiveCoursesCount = category.ActiveCoursesCount
            };
        }

        private static CourseCategoryDto MapToDtoWithCourses(CourseCategory category)
        {
            var dto = MapToDto(category);
            dto.Courses = category.Courses?.Select(c => new CourseDto
            {
                CourseId = c.CourseId,
                Title = c.Title,
                Description = c.Description,
                IsPublished = c.IsPublished,
                Price = c.Price,
                IsFree = c.IsFree,
                InstructorId = c.InstructorId,
                CreatedAt = c.CreatedAt
            }).ToList() ?? new List<CourseDto>();

            return dto;
        }

        #endregion
    }
}