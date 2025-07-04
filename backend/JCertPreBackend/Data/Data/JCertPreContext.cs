// backend/JCertPreBackend/Data/JCertPreContext.cs
using Microsoft.EntityFrameworkCore;
using JCertPreBackend.Models;

namespace JCertPreBackend.Data
{
    public class JCertPreContext : DbContext
    {
        public JCertPreContext(DbContextOptions<JCertPreContext> options) : base(options) { }
        
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Admin> Admins { get; set; } = null!;
        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Quiz> Quizzes { get; set; } = null!;
        public DbSet<Question> Questions { get; set; } = null!;
        public DbSet<TestResult> TestResults { get; set; } = null!;
        public DbSet<UserCourse> UserCourses { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure table names (MySQL naming convention)
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Admin>().ToTable("admins");
            modelBuilder.Entity<Course>().ToTable("courses");
            modelBuilder.Entity<Quiz>().ToTable("quizzes");
            modelBuilder.Entity<Question>().ToTable("questions");
            modelBuilder.Entity<TestResult>().ToTable("test_results");
            modelBuilder.Entity<UserCourse>().ToTable("user_courses");

            // Configure column types for MySQL
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("Student");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP()");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Admin>(entity =>
            {
                entity.Property(e => e.Username).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Password).HasMaxLength(255).IsRequired();
                entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Email).HasMaxLength(150).IsRequired();
                entity.Property(e => e.Role).HasMaxLength(20).HasDefaultValue("Admin");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP()");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
                entity.HasIndex(e => e.Username).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();
            });

            modelBuilder.Entity<Course>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Type).HasMaxLength(50).IsRequired();
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.VideoUrl).HasMaxLength(500);
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP()");
                entity.Property(e => e.UpdatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP() ON UPDATE CURRENT_TIMESTAMP()");
            });

            modelBuilder.Entity<Quiz>(entity =>
            {
                entity.Property(e => e.Title).HasMaxLength(200).IsRequired();
                entity.Property(e => e.Description).HasColumnType("text");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP()");
            });

            modelBuilder.Entity<Question>(entity =>
            {
                entity.Property(e => e.Text).HasColumnType("text").IsRequired();
                entity.Property(e => e.Options).HasColumnType("json").IsRequired();
                entity.Property(e => e.CorrectAnswer).HasMaxLength(255).IsRequired();
            });

            modelBuilder.Entity<TestResult>(entity =>
            {
                entity.Property(e => e.Answers).HasColumnType("json");
                entity.Property(e => e.CompletedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP()");
            });

            modelBuilder.Entity<UserCourse>(entity =>
            {
                entity.Property(e => e.EnrolledAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP()");
                entity.Property(e => e.LastAccessedAt).HasColumnType("datetime").HasDefaultValueSql("CURRENT_TIMESTAMP() ON UPDATE CURRENT_TIMESTAMP()");
            });

            // Configure relationships
            modelBuilder.Entity<Quiz>()
                .HasOne(q => q.Course)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Question>()
                .HasOne(q => q.Quiz)
                .WithMany(qz => qz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestResult>()
                .HasOne(tr => tr.User)
                .WithMany(u => u.TestResults)
                .HasForeignKey(tr => tr.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TestResult>()
                .HasOne(tr => tr.Quiz)
                .WithMany(qz => qz.TestResults)
                .HasForeignKey(tr => tr.QuizId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.User)
                .WithMany(u => u.UserCourses)
                .HasForeignKey(uc => uc.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserCourse>()
                .HasOne(uc => uc.Course)
                .WithMany(c => c.UserCourses)
                .HasForeignKey(uc => uc.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}