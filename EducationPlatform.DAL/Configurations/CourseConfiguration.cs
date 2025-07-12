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
    public class CourseConfiguration : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> builder)
        {
            // Table Configuration
            builder.ToTable("Courses");

            // Primary Key
            builder.HasKey(c => c.CourseId);

            // Basic Properties
            builder.Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(c => c.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(c => c.IsPublished)
                .IsRequired()
                .HasDefaultValue(false);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CourseStatus.Draft);

            builder.Property(c => c.Difficulty)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50)
                .HasDefaultValue(CourseDifficulty.Beginner);

            // Date Properties
            builder.Property(c => c.CreatedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(c => c.LastModifiedAt)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(c => c.PublishedAt)
                .IsRequired(false);

            builder.Property(c => c.StartDate)
                .IsRequired(false);

            builder.Property(c => c.EndDate)
                .IsRequired(false);

            // Numeric Properties
            builder.Property(c => c.EstimatedDuration)
                .IsRequired(false);

            builder.Property(c => c.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired(false);

            builder.Property(c => c.IsFree)
                .IsRequired()
                .HasDefaultValue(false);

            // String Properties
            builder.Property(c => c.ThumbnailUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.PreviewVideoUrl)
                .HasMaxLength(500)
                .IsRequired(false);

            builder.Property(c => c.Requirements)
                .HasMaxLength(4000)
                .IsRequired(false);

            builder.Property(c => c.LearningObjectives)
                .HasMaxLength(4000)
                .IsRequired(false);

           

            // Audit Fields
            builder.Property(c => c.CreatedUtc)
                .IsRequired()
                .HasDefaultValue(DateTime.UtcNow);

            builder.Property(c => c.CreatedBy)
                .IsRequired()
                .HasMaxLength(256)
                .HasDefaultValue("MohamadEnma");

            builder.Property(c => c.ModifiedUtc)
                .IsRequired(false);

            builder.Property(c => c.ModifiedBy)
                .HasMaxLength(256)
                .IsRequired(false);

            // Relationships
            builder.HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            //// Collections
            //builder.HasMany(c => c.Lessons)
            //    .WithOne(l => l.Course)
            //    .HasForeignKey(l => l.CourseId)
            //    .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(c => c.Enrollments)
                .WithOne(e => e.Course)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            //builder.HasMany(c => c.Materials)
            //    .WithOne(m => m.Course)
            //    .HasForeignKey(m => m.CourseId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(c => c.Discussions)
            //    .WithOne(d => d.Course)
            //    .HasForeignKey(d => d.CourseId)
            //    .OnDelete(DeleteBehavior.Cascade);



            // Indexes
            builder.HasIndex(c => c.Title)
                .HasDatabaseName("IX_Courses_Title");

            builder.HasIndex(c => c.Status)
                .HasDatabaseName("IX_Courses_Status");

            builder.HasIndex(c => c.InstructorId)
                .HasDatabaseName("IX_Courses_InstructorId");

            builder.HasIndex(c => c.CategoryId)
                .HasDatabaseName("IX_Courses_CategoryId");

            builder.HasIndex(c => c.CreatedUtc)
                .HasDatabaseName("IX_Courses_CreatedUtc");

            builder.HasIndex(c => new { c.IsPublished, c.Status })
                .HasDatabaseName("IX_Courses_PublishStatus");

            // Query Filters
            builder.HasQueryFilter(c => !c.IsDeleted); // If you have soft delete



        }
    }
}
