// backend/JCertPreBackend/Models/Course.cs
namespace JCertPreBackend.Models
{
    public class Course
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public List<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public List<UserCourse> UserCourses { get; set; } = new List<UserCourse>();
    }
}