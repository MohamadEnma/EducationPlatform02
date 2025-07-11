using EducationPlatform.DAL.IRepositories;
using EducationPlatform.DAL.Models.CourseModels;
using EducationPlatform.DAL.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EducationPlatform.DAL.Data
{
    public class EducationDbContext : DbContext
    {
        public EducationDbContext(DbContextOptions<EducationDbContext> options) : base(options)
        {
        }

        public DbSet<ApplicationUser> Students { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<CourseCategory> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision for financial and percentage fields
            modelBuilder.Entity<Course>()
                .Property(c => c.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.CompletionPercentage)
                .HasColumnType("decimal(5,2)"); // 0.00 to 100.00

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.FinalGrade)
                .HasColumnType("decimal(5,2)"); // 0.00 to 100.00

            modelBuilder.Entity<Enrollment>()
                .Property(e => e.PaidAmount)
                .HasColumnType("decimal(18,2)");

            // Fix foreign key cascade conflicts
            // Students -> Courses (Instructor relationship) should use NO ACTION to prevent cascade conflicts
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.Courses)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.NoAction);

            // Students -> Enrollments should use NO ACTION
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.NoAction);

            // Courses -> Enrollments can keep CASCADE as it's a direct parent-child relationship
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Categories -> Courses should use SET NULL
            modelBuilder.Entity<Course>()
                .HasOne(c => c.Category)
                .WithMany()
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}