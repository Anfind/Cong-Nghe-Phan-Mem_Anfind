// backend/JCertPreBackend/Models/User.cs
namespace JCertPreBackend.Models
{
    public class User
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; } // Demo, không mã hóa
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? LastLoginAt { get; set; }

        // Navigation properties
        public List<TestResult> TestResults { get; set; } = new List<TestResult>();
        public List<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}