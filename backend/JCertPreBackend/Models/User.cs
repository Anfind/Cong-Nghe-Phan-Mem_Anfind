// backend/JCertPreBackend/Models/User.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty; // Sẽ được hash với BCrypt
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        
        public string? Role { get; set; } = "Student"; // Student, Admin
        public bool IsActive { get; set; } = true;
        public bool IsEmailVerified { get; set; } = false;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public List<TestResult> TestResults { get; set; } = new List<TestResult>();
        public List<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}