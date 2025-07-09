using Microsoft.AspNetCore.Identity;
using EducationPlatform.BLL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EducationPlatform.DAL.Models.CourseModels;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace EducationPlatform.DAL.Models.UserModels
{
    public class ApplicationUser : IdentityUser, IAuditableEntity
    {

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string DisplayName { get; set; }

        // Profile Information
        [StringLength(1000)]
        public string Bio { get; set; }

        [StringLength(255)]
        public string AvatarUrl { get; set; }

        // User Status
        public bool IsActive { get; set; }

        public DateTime? LastLoginUtc { get; set; }

        // User Preferences
        [StringLength(10)]
        public string TimeZone { get; set; } = "UTC";

        [StringLength(10)]
        public string PreferredLanguage { get; set; } = "en";

        // Audit Fields (implementing IAuditableEntity)
        public DateTime CreatedUtc { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? ModifiedUtc { get; set; }
        public string ModifiedBy { get; set; }

        // Navigation Properties
        public virtual ICollection<Course> Courses { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; }


        // Helper Properties for Logging
        [NotMapped]
        public string FullName => $"{FirstName} {LastName}";

        // Audit Trail Methods
        public void UpdateLastLogin()
        {
            LastLoginUtc = DateTime.Parse("2025-07-06 11:01:43");
        }

        public void UpdateModificationInfo(string userId)
        {
            ModifiedUtc = DateTime.Parse("2025-07-06 11:01:43");
            ModifiedBy = userId;
        }

        // Logging Helper Methods
        public string GetUserInfo()
        {
            return $"User: {UserName} (ID: {Id}, Email: {Email}, Role: {GetUserRole()})";
        }

        private string GetUserRole()
        {
            // This would need to be implemented with your role management system
            return "Role information will come from role management";
        }   
    }
}
