// backend/JCertPreBackend/DTOs/AdminDto.cs
using System.ComponentModel.DataAnnotations;

namespace JCertPreBackend.DTOs
{
    public class AdminLoginDto
    {
        [Required]
        public string Username { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }

    public class AdminResponseDto
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsSuper { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        
        // Permissions
        public bool CanManageCourses { get; set; }
        public bool CanManageUsers { get; set; }
        public bool CanManageQuizzes { get; set; }
        public bool CanViewReports { get; set; }
    }

    public class DashboardStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCourses { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalTestResults { get; set; }
        public int ActiveUsers { get; set; }
        public int PublishedCourses { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<CourseStatsDto> TopCourses { get; set; } = new();
        public List<UserActivityDto> RecentActivities { get; set; } = new();
    }

    public class CourseStatsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int EnrolledUsers { get; set; }
        public int ViewCount { get; set; }
        public decimal Revenue { get; set; }
    }

    public class UserActivityDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
    }
}
