// backend/JCertPreBackend/Models/Course.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.Models
{
    public class Course
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;
        
        public string? Description { get; set; }
        
        [MaxLength(500)]
        public string? ImageUrl { get; set; }
        
        [MaxLength(500)]
        public string? VideoUrl { get; set; }
        
        public bool IsActive { get; set; } = true;
        public bool IsPublished { get; set; } = false;
        public int ViewCount { get; set; } = 0;
        public decimal Price { get; set; } = 0;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public List<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}