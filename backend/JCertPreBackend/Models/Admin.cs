// backend/JCertPreBackend/Models/Admin.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.Models
{
    public class Admin
    {
        public int Id { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        [MaxLength(255)]
        public string Password { get; set; } = string.Empty; // Hashed with BCrypt
        
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        
        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;
        
        public string Role { get; set; } = "Admin";
        public bool IsActive { get; set; } = true;
        public bool IsSuper { get; set; } = false; // Super admin
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Permissions
        public bool CanManageCourses { get; set; } = true;
        public bool CanManageUsers { get; set; } = true;
        public bool CanManageQuizzes { get; set; } = true;
        public bool CanViewReports { get; set; } = true;
    }
}
