using EducationPlatform.DAL.Models.UserModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Models.CourseModels
{
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        // Relationships
        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = new Course();

        [Required]
        public string StudentId { get; set; } = string.Empty;

        [ForeignKey("StudentId")]
        public virtual ApplicationUser Student { get; set; } = new ApplicationUser();

        // Enrollment Status
        [Required]
        public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

        public bool IsActive { get; set; }

        // Dates
        [Required]
        public DateTime EnrollmentDate { get; set; }

        public DateTime? CompletionDate { get; set; }

        public DateTime? ExpiryDate { get; set; }

        public DateTime? LastAccessedAt { get; set; }

        // Progress Tracking
        public decimal CompletionPercentage { get; set; }

        public int CompletedLessons { get; set; }

        public int TotalLessons { get; set; }

        public TimeSpan? TotalTimeSpent { get; set; }

        // Grade Information
        public decimal? FinalGrade { get; set; }

        public bool HasPassed { get; set; }

        public string GradeComments { get; set; } = string.Empty;

        // Payment Information
        public decimal? PaidAmount { get; set; }

        [StringLength(50)]
        public string PaymentStatus { get; set; } = string.Empty;

        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        public DateTime? PaymentDate { get; set; }

        // Certificate
        public bool CertificateIssued { get; set; }

        public DateTime? CertificateIssuedDate { get; set; }

        [StringLength(100)]
        public string CertificateNumber { get; set; } = string.Empty;

        [StringLength(500)]
        public string CertificateUrl { get; set; } = string.Empty;

        // Collections


        // Additional Settings
        public bool NotificationsEnabled { get; set; } = true;

        [StringLength(50)]
        public string PreferredLanguage { get; set; } = string.Empty;

        // Notes and Comments
        [StringLength(1000)]
        public string Notes { get; set; } = string.Empty;

        [StringLength(1000)]
        public string AdminComments { get; set; } = string.Empty;

        // Audit Fields
        [Required]
        public DateTime CreatedUtc { get; set; }

        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; } = string.Empty; 

        public DateTime? ModifiedUtc { get; set; }

        [StringLength(256)]
        public string ModifiedBy { get; set; } = string.Empty; 

        [Required]
        public DateTime LastModifiedAt { get; set; }

        // Soft Delete
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string DeletedBy { get; set; } = string.Empty;

        // Helper Methods
        public bool IsExpired()
        {
            if (!ExpiryDate.HasValue) return false;
            return DateTime.Parse("2025-07-06 11:35:15") > ExpiryDate.Value;
        }

        public void UpdateProgress(int completedLessons, int totalLessons)
        {
            CompletedLessons = completedLessons;
            TotalLessons = totalLessons;
            CompletionPercentage = totalLessons == 0 ? 0 : (decimal)completedLessons / totalLessons * 100;

            if (CompletionPercentage >= 100)
            {
                CompletionDate = DateTime.UtcNow;
                Status = EnrollmentStatus.Completed;
            }
        }

        public void UpdateLastAccessed()
        {
            LastAccessedAt = DateTime.UtcNow;
        }

        public void IssueCertificate()
        {
            if (!CertificateIssued && HasPassed)
            {
                CertificateIssued = true;
                CertificateIssuedDate = DateTime.UtcNow;
                CertificateNumber = GenerateCertificateNumber();
            }
        }

        private string GenerateCertificateNumber()
        {
            return $"CERT-{CourseId}-{StudentId}-{DateTime.UtcNow:yyyyMMdd}-{EnrollmentId}";
        }
    }

    public enum EnrollmentStatus
    {
        Pending,
        Active,
        Completed,
        Withdrawn,
        Suspended,
        Expired,
        Dropped
    }
}

