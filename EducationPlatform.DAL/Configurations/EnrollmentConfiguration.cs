using EducationPlatform.DAL.Models.CourseModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Configurations
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");

            // Primary Key
            builder.HasKey(e => e.EnrollmentId);


            // Properties
            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(e => e.CompletionPercentage)
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.FinalGrade)
                .HasColumnType("decimal(5,2)");

            builder.Property(e => e.PaidAmount)
                .HasColumnType("decimal(10,2)");

            builder.Property(e => e.PaymentStatus)
                .HasMaxLength(50);

            builder.Property(e => e.TransactionId)
                .HasMaxLength(100);

            builder.Property(e => e.CertificateNumber)
                .HasMaxLength(100);

            builder.Property(e => e.CertificateUrl)
                .HasMaxLength(500);

            builder.Property(e => e.PreferredLanguage)
                .HasMaxLength(50);

            builder.Property(e => e.Notes)
                .HasMaxLength(1000);

            builder.Property(e => e.AdminComments)
                .HasMaxLength(1000);

            // Audit Fields
            builder.Property(e => e.CreatedUtc)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(e => e.CreatedBy)
                .IsRequired()
                .HasMaxLength(256)
                .HasDefaultValue("MohamadEnma");

            builder.Property(e => e.LastModifiedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            // Relationships
            builder.HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(e => new { e.CourseId, e.StudentId })
                .HasDatabaseName("IX_Enrollments_Course_Student")
                .IsUnique();

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Enrollments_Status");

            builder.HasIndex(e => e.EnrollmentDate)
                .HasDatabaseName("IX_Enrollments_EnrollmentDate");

            builder.HasIndex(e => e.CompletionDate)
                .HasDatabaseName("IX_Enrollments_CompletionDate");

            builder.HasIndex(e => e.LastAccessedAt)
                .HasDatabaseName("IX_Enrollments_LastAccessedAt");

            builder.HasIndex(e => e.PaymentStatus)
                .HasDatabaseName("IX_Enrollments_PaymentStatus");

            // Query Filter
            builder.HasQueryFilter(e => !e.IsDeleted);

        }
    }
}
